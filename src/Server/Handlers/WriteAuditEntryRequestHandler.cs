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
    public class WriteAuditEntryRequestHandler : RequestHandler<WriteAuditEntryRequest, WriteAuditEntryResponse>
    {
        public ISession Session { get; set; }

        protected override void Handle()
        {
            var auditEntryModel = new AuditEntryModel();
            auditEntryModel.InjectFrom(Request);

            Session.Save(auditEntryModel);

            Response.Id = auditEntryModel.Id;
        }
    }
}
