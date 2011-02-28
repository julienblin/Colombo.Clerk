using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Queries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Omu.ValueInjecter;

namespace Colombo.Clerk.Server.Handlers
{
    public class GetAuditEntriesForStreamRequestHandler : SideEffectFreeRequestHandler<GetAuditEntriesForStreamRequest, GetAuditEntriesForStreamResponse>
    {
        private readonly ISession session;

        public GetAuditEntriesForStreamRequestHandler(ISession session)
        {
            this.session = session;
        }

        protected override void Handle()
        {
            var streamModel = session.QueryOver<StreamModel>()
                .Where(x => x.Id == Request.Id)
                .Fetch(x => x.Filters).Eager
                .TransformUsing(Transformers.DistinctRootEntity)
                .List().FirstOrDefault();

            if (streamModel == null)
                throw new Exception(string.Format("Unable to find stream {0}", Request.Id));

            var auditEntryQuery = new AuditEntriesFromFiltersQuery { Filters = streamModel.Filters };

            var paginationResult = session.PaginateQuery(auditEntryQuery, Request);

            var results = session.QueryOver<AuditEntryModel>()
                .WithSubquery.WhereProperty(x => x.Id).In(
                    paginationResult.PaginatedQuery
                        .OrderBy(x => x.RequestUtcTimestamp).Desc
                        .Select(Projections.Id())
                )
                .TransformUsing(Transformers.DistinctRootEntity)
                .Fetch(x => x.Context).Eager
                .Future();

            SetPaginationInfo(paginationResult.RowCount.Value);

            Response.Results = new List<AuditEntry>(
                results.Select(x =>
                {
                    var ae = new AuditEntry();
                    ae.InjectFrom<UnflatLoopValueInjection>(x);
                    if (x.Context != null)
                        foreach (var contextEntry in x.Context)
                            if (contextEntry.ContextKey != null)
                                ae.RequestContext[contextEntry.ContextKey] = contextEntry.ContextValue;
                    return ae;
                })
            );
        }
    }
}
