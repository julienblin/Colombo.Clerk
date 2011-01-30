using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class GetAuditEntryByIdRequest : SideEffectFreeRequest<AuditEntryResponse>
    {
        public Guid Id { get; set; }
    }
}
