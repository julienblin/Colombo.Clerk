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
        }
    }
}