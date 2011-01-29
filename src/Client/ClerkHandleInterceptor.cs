using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Castle.Core.Logging;
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

        private readonly IClerkServiceFactory clerkServiceFactory;

        public ClerkHandleInterceptor(IClerkServiceFactory clerkServiceFactory)
        {
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
                                            Serialized = Serialize(nextInvocation.Request)
                                        },
                                    Context = nextInvocation.Request.Context,
                                    ServerMachineName = Environment.MachineName
                                };

            try
            {
                nextInvocation.Proceed();
                var responseType = nextInvocation.Response.GetType();
                auditInfo.Response.Namespace = responseType.Namespace;
                auditInfo.Response.Type = responseType.Name;
                auditInfo.Response.Serialized = Serialize(nextInvocation.Response);
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
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
                            clerkService.Write(auditInfo);
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
