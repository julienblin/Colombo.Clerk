using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class GetAuditEntryByIdResponse : Response
    {
        public virtual bool Found { get; set; }

        public virtual AuditEntry AuditEntry { get; set; }
    }
}
