using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using Omu.ValueInjecter;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Queries;
using NHibernate;

namespace Colombo.Clerk.Server.Handlers
{
    public class SearchAuditEntryRequestHandler : SideEffectFreeRequestHandler<SearchAuditEntryRequest, SearchAuditEntryResponse>
    {
        public ISession Session { get; set; }

        protected override void Handle()
        {
            var auditEntryQuery = new AuditEntrySearchQuery();
            auditEntryQuery.InjectFrom(Request);

            var rowCount = auditEntryQuery.GetQuery().GetExecutableQueryOver(Session)
                .Select(Projections.RowCount())
                .FutureValue<Int32>();


            var results = auditEntryQuery.GetQuery().GetExecutableQueryOver(Session)
                .Take(Request.PerPage)
                .Skip(Request.PerPage * Request.CurrentPage)
                .Future();

            Response.CurrentPage = Request.CurrentPage;
            Response.PerPage = Request.PerPage;
            Response.TotalEntries = rowCount.Value;
            Response.Results = new List<AuditEntry>(
                results.Select(x =>
                {
                    var ae = new AuditEntry();
                    ae.InjectFrom<UnflatLoopValueInjection>(x);
                    return ae;
                })
            );
        }
    }
}
