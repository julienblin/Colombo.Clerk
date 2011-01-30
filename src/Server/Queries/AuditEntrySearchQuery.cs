using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using NHibernate;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Queries
{
    public class AuditEntrySearchQuery
    {
        public string RequestNamespaceLike { get; set; }

        public QueryOver<AuditEntryModel, AuditEntryModel> GetQuery()
        {
            var queryOver = QueryOver.Of<AuditEntryModel>();

            if (!string.IsNullOrWhiteSpace(RequestNamespaceLike))
                queryOver.Where(Restrictions.On<AuditEntryModel>(c => c.RequestNamespace).IsLike(RequestNamespaceLike,
                                                                                                 MatchMode.Anywhere));

            return queryOver;
        }
    }
}
