using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class SearchAuditEntryResponse : Response
    {
        public SearchAuditEntryResponse()
        {
            Results = new List<AuditEntry>();
        }

        public virtual int TotalEntries { get; set; }

        public virtual int CurrentPage { get; set; }

        public virtual int PerPage { get; set; }

        public IList<AuditEntry> Results { get; set; }
    }
}
