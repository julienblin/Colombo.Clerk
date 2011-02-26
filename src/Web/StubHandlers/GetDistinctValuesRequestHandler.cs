using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Colombo.Clerk.Messages;

namespace Colombo.Clerk.Web.StubHandlers
{
    public class GetDistinctValuesRequestHandler : SideEffectFreeRequestHandler<GetDistinctValuesRequest, GetDistinctValuesResponse>
    {
        protected override void Handle()
        {
            Response.Values = new List<string> { "server1", "server2" };
        }
    }
}