using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using AutoMapper;
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
            var auditEntry = Mapper.Map<AuditInfo, AuditEntry>(auditInfo);

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
    }
}
