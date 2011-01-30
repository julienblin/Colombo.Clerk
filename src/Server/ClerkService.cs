using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using Omu.ValueInjecter;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Service;
using NHibernate;

namespace Colombo.Clerk.Server
{
    public class ClerkService : IClerkService
    {
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Write(AuditInfo auditInfo)
        {
            var auditEntry = new AuditEntryModel();
            auditEntry.InjectFrom<FlatLoopValueInjection>(auditInfo);

            StripLongStrings(auditEntry);

            ISession session = null;
            try
            {
                session = EndPointConfig.Kernel.Resolve<ISession>();
                session.Save(auditEntry);
            }
            finally
            {
                if(session != null)
                    EndPointConfig.Kernel.ReleaseComponent(session);
            }
        }

        private static void StripLongStrings(AuditEntryModel auditEntry)
        {
            if ((auditEntry.RequestNamespace != null) && (auditEntry.RequestNamespace.Length > 255))
                auditEntry.RequestNamespace = auditEntry.RequestNamespace.Substring(0, 255);

            if ((auditEntry.RequestType != null) && (auditEntry.RequestType.Length > 255))
                auditEntry.RequestType = auditEntry.RequestType.Substring(0, 255);

            if ((auditEntry.ResponseNamespace != null) && (auditEntry.ResponseNamespace.Length > 255))
                auditEntry.ResponseNamespace = auditEntry.ResponseNamespace.Substring(0, 255);

            if ((auditEntry.ResponseType != null) && (auditEntry.ResponseType.Length > 255))
                auditEntry.ResponseType = auditEntry.ResponseType.Substring(0, 255);

            if ((auditEntry.ServerMachineName != null) && (auditEntry.ServerMachineName.Length > 255))
                auditEntry.ServerMachineName = auditEntry.ServerMachineName.Substring(0, 255);
        }
    }
}
