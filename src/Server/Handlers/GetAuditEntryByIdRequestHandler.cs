using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omu.ValueInjecter;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using NHibernate;

namespace Colombo.Clerk.Server.Handlers
{
    public class GetAuditEntryByIdRequestHandler : SideEffectFreeRequestHandler<GetAuditEntryByIdRequest, GetAuditEntryByIdResponse>
    {
        public ISession Session { get; set; }

        protected override void Handle()
        {
            var auditEntryModel = Session.Get<AuditEntryModel>(Request.Id);

            if (auditEntryModel != null)
            {
                Response.Found = true;
                Response.AuditEntry = new AuditEntry();
                Response.AuditEntry.InjectFrom<UnflatLoopValueInjection>(auditEntryModel);
            }
        }
    }
}
