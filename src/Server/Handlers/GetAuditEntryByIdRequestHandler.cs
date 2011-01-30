using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using NHibernate;

namespace Colombo.Clerk.Server.Handlers
{
    public class GetAuditEntryByIdRequestHandler : SideEffectFreeRequestHandler<GetAuditEntryByIdRequest, AuditEntryResponse>
    {
        public ISession Session { get; set; }

        protected override void Handle()
        {
            var auditEntry = Session.Get<AuditEntry>(Request.Id);

            if (auditEntry != null)
            {
                Mapper.Map(auditEntry, Response);
            }
        }
    }
}
