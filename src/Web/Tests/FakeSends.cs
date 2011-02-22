using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Colombo.Clerk.Messages;
using Colombo.TestSupport;

namespace Colombo.Clerk.Web.Tests
{
    public static class FakeSends
    {
        public static void ConfigureFakeSends(this IStubMessageBus stubMessageBus)
        {
            stubMessageBus.ExpectSend<GetDistinctValuesRequest, GetDistinctValuesResponse>().Reply(
            (request, response) =>
            {
                response.Values = new List<string> { "server1", "server2" };
            });

            stubMessageBus.ExpectSend<GetStatsByServerRequest, GetStatsByServerResponse>().Reply(
            (request, response) =>
            {
                response.ServerStats = new Dictionary<string, ServerStats>
                {
                    { "server1", new ServerStats { NumRequestsSent = 20, NumRequestsHandled = 30 } },
                    { "server2", new ServerStats { NumRequestsSent = 0, NumRequestsHandled = 150 } }
                };
            });
        }
    }
}