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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Colombo.Clerk.Server.Models;
using NHibernate;

namespace Colombo.Clerk.Server.Interceptors
{
    public class InternalAuditRequestHandlerInterceptor : IRequestHandlerHandleInterceptor
    {
        public ISessionFactory SessionFactory { get; set; }

        public void Intercept(IColomboRequestHandleInvocation nextInvocation)
        {
            var requestType = nextInvocation.Request.GetType();

            var auditEntryModel = new AuditEntryModel
            {
                RequestNamespace = requestType.Namespace,
                RequestType = requestType.Name,
                RequestCorrelationGuid = nextInvocation.Request.CorrelationGuid,
                RequestSerialized = Serialize(nextInvocation.Request),
                RequestUtcTimestamp = nextInvocation.Request.UtcTimestamp,
                Context = new List<ContextEntryModel>(
                    nextInvocation.Request.Context.Select(kv => new ContextEntryModel { ContextKey = kv.Key, ContextValue = kv.Value })
                )
            };

            nextInvocation.Proceed();

            var responseType = nextInvocation.Response.GetType();
            auditEntryModel.ResponseNamespace = responseType.Namespace;
            auditEntryModel.ResponseType = responseType.Name;
            auditEntryModel.ResponseCorrelationGuid = nextInvocation.Response.CorrelationGuid;
            auditEntryModel.ResponseSerialized = Serialize(nextInvocation.Response);
            auditEntryModel.ResponseUtcTimestamp = nextInvocation.Response.UtcTimestamp;

            using(var session = SessionFactory.OpenSession())
            using(var tx = session.BeginTransaction())
            {
                session.Save(auditEntryModel);
                tx.Commit();
            }
        }

        public int InterceptionPriority
        {
            get { return InterceptionPrority.Medium; }
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
