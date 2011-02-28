using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Queries
{
    public class AuditEntriesFromFiltersQuery : IQuery<AuditEntryModel>
    {
        public AuditEntriesFromFiltersQuery()
        {
            Filters = new List<FilterModel>();
        }

        public IList<FilterModel> Filters { get; set; }

        public QueryOver<AuditEntryModel> GetQuery()
        {
            AuditEntryModel auditEntryModelAlias = null;
            var queryOver = QueryOver.Of(() => auditEntryModelAlias);

            if(Filters == null)
                Filters = new List<FilterModel>();

            foreach (var filterModel in Filters)
            {
                switch (filterModel.FilterName)
                {
                    case "RequestNamespaceFilter":
                        queryOver.Where(x => x.RequestNamespace == filterModel.StringValue);
                        break;
                    case "RequestTypeFilter":
                        queryOver.Where(x => x.RequestType == filterModel.StringValue);
                        break;
                    case "RequestCorrelationGuidFilter":
                        queryOver.Where(x => x.RequestCorrelationGuid == filterModel.GuidValue);
                        break;
                    case "ResponseNamespaceFilter":
                        queryOver.Where(x => x.ResponseNamespace == filterModel.StringValue);
                        break;
                    case "ResponseTypeFilter":
                        queryOver.Where(x => x.ResponseType == filterModel.StringValue);
                        break;
                    case "ResponseCorrelationGuidFilter":
                        queryOver.Where(x => x.ResponseCorrelationGuid == filterModel.GuidValue);
                        break;
                    case "HasExceptionFilter":
                        if (filterModel.BoolValue.Value)
                            queryOver.Where(x => (!(x.Exception == null || x.Exception == "")));
                        else
                            queryOver.Where(x => (x.Exception == null || x.Exception == ""));
                        break;
                    case "RequestUtcTimestampAfterFilter":
                        queryOver.Where(x => x.RequestUtcTimestamp >= filterModel.DateTimeValue);
                        break;
                    case "ExceptionContainsFilter":
                        queryOver.Where(Restrictions.On<AuditEntryModel>(r => r.Exception).IsLike(filterModel.StringValue, MatchMode.Anywhere));
                        break;
                    case "MessageContainsFilter":
                        queryOver.Where(Restrictions.On<AuditEntryModel>(r => r.Message).IsLike(filterModel.StringValue, MatchMode.Anywhere));
                        break;
                    case "HasMessageFilter":
                        if (filterModel.BoolValue.Value)
                            queryOver.Where(x => (!(x.Message == null || x.Message == "")));
                        else
                            queryOver.Where(x => (x.Message == null || x.Message == ""));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return queryOver;
        }
    }
}
