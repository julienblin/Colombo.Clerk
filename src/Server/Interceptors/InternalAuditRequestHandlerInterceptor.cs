using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Castle.MicroKernel;
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
