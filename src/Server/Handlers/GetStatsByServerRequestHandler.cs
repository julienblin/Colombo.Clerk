#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Queries;
using Colombo.Clerk.Server.Services;
using NHibernate;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Handlers
{
    public class GetStatsByServerRequestHandler : SideEffectFreeRequestHandler<GetStatsByServerRequest, GetStatsByServerResponse>
    {
        private readonly ISession session;

        private readonly IClock clock;

        public GetStatsByServerRequestHandler(ISession session, IClock clock)
        {
            this.session = session;
            this.clock = clock;
        }

        protected override void Handle()
        {
            var distinctMachineNames = GetDistinctMachineNames();
            var futureServerStats = GetServerStats(distinctMachineNames, clock.UtcNow.Subtract(Request.Since));

            foreach (var distinctMachineName in distinctMachineNames)
            {
                Response.ServerStats[distinctMachineName] = new ServerStats
                {
                    NumRequestsSent = futureServerStats[distinctMachineName][0].Value,
                    NumRequestsHandled = futureServerStats[distinctMachineName][1].Value
                };
            }
        }

        private IDictionary<string, IList<IFutureValue<int>>> GetServerStats(IEnumerable<string> machineNames, DateTime since)
        {
            var result = new Dictionary<string, IList<IFutureValue<int>>>();

            foreach (var machineName in machineNames)
            {
                var querySent = new AuditEntrySearchQuery
                {
                    ContextConditions = new List<AuditEntrySearchQuery.ContextCondition>
                    {
                        new AuditEntrySearchQuery.ContextCondition { Key = MetaContextKeys.SenderMachineName, ValueIs = machineName}
                    },
                    RequestUtcTimestampAfter = since
                };
                var queryHandled = new AuditEntrySearchQuery
                {
                    ContextConditions = new List<AuditEntrySearchQuery.ContextCondition>
                    {
                        new AuditEntrySearchQuery.ContextCondition { Key = MetaContextKeys.HandlerMachineName, ValueIs = machineName}
                    },
                    RequestUtcTimestampAfter = since
                };

                result[machineName] = new List<IFutureValue<int>>();
                result[machineName].Add(querySent.GetQuery().GetExecutableQueryOver(session).Select(Projections.RowCount()).FutureValue<Int32>());
                result[machineName].Add(queryHandled.GetQuery().GetExecutableQueryOver(session).Select(Projections.RowCount()).FutureValue<Int32>());
            }
            return result;
        }

        private IEnumerable<string> GetDistinctMachineNames()
        {
            var query = session.QueryOver<ContextEntryModel>()
                            .Where(x => (x.ContextKey == MetaContextKeys.SenderMachineName) || (x.ContextKey == MetaContextKeys.HandlerMachineName))
                            .Select(Projections.Distinct(Projections.Property<ContextEntryModel>(x => x.ContextValue)))
                            .OrderBy(x => x.ContextValue).Asc;
            return query.List<string>();
        }
    }
}
