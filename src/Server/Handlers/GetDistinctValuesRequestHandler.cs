using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using NHibernate;
using NHibernate.Criterion;
using System.Linq.Expressions;

namespace Colombo.Clerk.Server.Handlers
{
    public class GetDistinctValuesRequestHandler : SideEffectFreeRequestHandler<GetDistinctValuesRequest, GetDistinctValuesResponse>
    {
        public ISession Session { get; set; }

        protected override void Handle()
        {
            if (Request.ValueType != GetDistinctValueType.ContextKey)
            {
                Expression<Func<AuditEntryModel, object>> expression = null;

                switch (Request.ValueType)
                {
                    case GetDistinctValueType.RequestNamespace:
                        expression = x => x.RequestNamespace;
                        break;
                    case GetDistinctValueType.RequestType:
                        expression = x => x.RequestType;
                        break;
                    case GetDistinctValueType.ResponseNamespace:
                        expression = x => x.ResponseNamespace;
                        break;
                    case GetDistinctValueType.ResponseType:
                        expression = x => x.ResponseType;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                var query = Session.QueryOver<AuditEntryModel>()
                                .Select(Projections.Distinct(Projections.Property(expression)))
                                .OrderBy(expression).Asc;

                Response.Values = query.List<string>();
            }
            else
            {
                var query = Session.QueryOver<ContextEntryModel>()
                                .Select(Projections.Distinct(Projections.Property<ContextEntryModel>(x => x.ContextKey)))
                                .OrderBy(x => x.ContextKey).Asc;

                Response.Values = query.List<string>();
            }
        }
    }
}
