using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Colombo.Clerk.Messages;

namespace Colombo.Clerk.Web.StubHandlers
{
    public class SearchAuditEntryRequestHandler : SideEffectFreeRequestHandler<SearchAuditEntryRequest, SearchAuditEntryResponse>
    {
        protected override void Handle()
        {
            SetPaginationInfo(0);
        }
    }
}