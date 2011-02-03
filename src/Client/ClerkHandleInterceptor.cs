#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Logging;
using Colombo.Clerk.Client.Alerts;
using Colombo.Clerk.Service;

namespace Colombo.Clerk.Client
{
    public class ClerkHandleInterceptor : IRequestHandlerHandleInterceptor
    {
        private ILogger logger = NullLogger.Instance;
        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        private IColomboAlerter[] alerters = new IColomboAlerter[0];
        /// <summary>
        /// Alerters to use. All will be notified.
        /// </summary>
        public IColomboAlerter[] Alerters
        {
            get { return alerters; }
            set
            {
                if (value == null) throw new ArgumentNullException("Alerters");
                Contract.EndContractBlock();

                alerters = value;
            }
        }

        private readonly IClerkServiceFactory clerkServiceFactory;

        public ClerkHandleInterceptor(IClerkServiceFactory clerkServiceFactory)
        {
            if (clerkServiceFactory == null) throw new ArgumentNullException("clerkServiceFactory");
            Contract.EndContractBlock();

            this.clerkServiceFactory = clerkServiceFactory;
        }

        public void Intercept(IColomboRequestHandleInvocation nextInvocation)
        {
            var requestType = nextInvocation.Request.GetType();

            var auditInfo = new AuditInfo
                                {
                                    Request =
                                        {
                                            Namespace = requestType.Namespace,
                                            Type = requestType.Name,
                                            CorrelationGuid = nextInvocation.Request.CorrelationGuid,
                                            Serialized = Serialize(nextInvocation.Request),
                                            UtcTimestamp = nextInvocation.Request.UtcTimestamp
                                        },
                                    Context = nextInvocation.Request.Context
                                };

            try
            {
                nextInvocation.Proceed();
                var responseType = nextInvocation.Response.GetType();
                auditInfo.Response.Namespace = responseType.Namespace;
                auditInfo.Response.Type = responseType.Name;
                auditInfo.Response.CorrelationGuid = nextInvocation.Response.CorrelationGuid;
                auditInfo.Response.Serialized = Serialize(nextInvocation.Response);
                auditInfo.Response.UtcTimestamp = nextInvocation.Response.UtcTimestamp;
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex.ToString();
                throw;
            }
            finally
            {
                Task.Factory.StartNew(() =>
                {
                    IClerkService clerkService = null;
                    try
                    {
                        clerkService = clerkServiceFactory.CreateClerkService();
                        if (clerkService != null)
                        {
                            clerkService.Write(auditInfo);
                        }
                        else
                        {
                            var alert = new ClerkServiceUnreachableAlert(auditInfo);
                            Logger.Warn(auditInfo.ToString());
                            foreach (var colomboAlerter in Alerters)
                            {
                                colomboAlerter.Alert(alert);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("An exception occured while writing clerk information", ex);
                    }
                    finally
                    {
                        if ((clerkService != null) && (clerkService is IClientChannel))
                        {
                            try
                            {
                                ((IClientChannel)clerkService).Close();
                            }
                            catch (Exception)
                            {
                                ((IClientChannel)clerkService).Abort();
                            }
                        }
                    }
                });
            }
        }

        public int InterceptionPriority
        {
            get { return InterceptionPrority.High; }
        }

        private static string Serialize(BaseMessage baseMessage)
        {
            using (var backing = new StringWriter())
            using (var writer = new XmlTextWriter(backing))
            {
                var serializer = new NetDataContractSerializer();
                serializer.WriteObject(writer, baseMessage);
                return backing.ToString();
            }
        }
    }
}
