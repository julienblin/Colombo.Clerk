using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Colombo.Clerk.Messages;

namespace Colombo.Clerk.Web.StubHandlers
{
    public class GetStatsByServerRequestHandler : SideEffectFreeRequestHandler<GetStatsByServerRequest, GetStatsByServerResponse>
    {
        protected override void Handle()
        {
            Response.ServerStats = new Dictionary<string, ServerStats>
                {
                    { "server1", new ServerStats { NumRequestsSent = 20, NumRequestsHandled = 30 } },
                    { "server2", new ServerStats { NumRequestsSent = 0, NumRequestsHandled = 150 } }
                };
        }
    }
}