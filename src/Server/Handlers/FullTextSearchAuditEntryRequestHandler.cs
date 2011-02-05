using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using NHibernate.Search;
using Omu.ValueInjecter;

namespace Colombo.Clerk.Server.Handlers
{
    public class FullTextSearchAuditEntryRequestHandler : SideEffectFreeRequestHandler<FullTextSearchAuditEntryRequest, SearchAuditEntryResponse>
    {
        public IFullTextSession FullTextSession { get; set; }

        protected override void Handle()
        {
            MultiFieldQueryParser queryParser = GetQueryParser();

            var query = FullTextSession.CreateFullTextQuery(queryParser.Parse(Request.SearchQuery), new[] { typeof(AuditEntryModel) })
                .SetFirstResult(Request.CurrentPage * Request.PerPage)
                .SetMaxResults(Request.PerPage);

            var results = query.List<AuditEntryModel>();

            Response.CurrentPage = Request.CurrentPage;
            Response.PerPage = Request.PerPage;
            Response.TotalEntries = query.ResultSize;
            Response.Results = new List<AuditEntry>(
                results.Select(x =>
                {
                    var ae = new AuditEntry();
                    ae.InjectFrom<UnflatLoopValueInjection>(x);
                    if (x.Context != null)
                        foreach (var contextEntry in x.Context)
                            ae.RequestContext[contextEntry.ContextKey] = contextEntry.ContextValue;
                    return ae;
                })
            );
        }

        private static MultiFieldQueryParser GetQueryParser()
        {
            return new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29,
                new string[]
                    {
                        "RequestNamespace",
                        "RequestType",
                        "RequestCorrelationGuid",
                        "ResponseNamespace",
                        "ResponseType",
                        "ResponseCorrelationGuid",
                        "Exception",
                        "Context.ContextKey",
                        "Context.ContextValue",
                    },
                new SimpleAnalyzer()
            );
        }
    }
}
