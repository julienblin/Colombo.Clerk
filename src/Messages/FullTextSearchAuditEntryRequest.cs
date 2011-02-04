using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class FullTextSearchAuditEntryRequest : SideEffectFreeRequest<SearchAuditEntryResponse>
    {
        public FullTextSearchAuditEntryRequest()
        {
            PerPage = 30;
        }

        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SearchQuery { get; set; }
    }
}
