using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class SearchAuditEntryRequest : SideEffectFreeRequest<SearchAuditEntryResponse>
    {
        public SearchAuditEntryRequest()
        {
            PerPage = 30;
        }

        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        public string RequestNamespaceLike { get; set; }
    }
}
