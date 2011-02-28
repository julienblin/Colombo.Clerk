using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class GetAuditEntriesForStreamResponse : PaginatedResponse
    {
        public GetAuditEntriesForStreamResponse()
        {
            Results = new List<AuditEntry>();
        }

        public virtual IList<AuditEntry> Results { get; set; }
    }
}
