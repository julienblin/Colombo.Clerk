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
using System.Linq;
using Colombo.Clerk.Server.Models;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Queries
{
    public class AuditEntrySearchQuery
    {
        public string RequestNamespace { get; set; }

        public string RequestContextContainsKey { get; set; }

        public string RequestType { get; set; }

        public Guid? RequestCorrelationGuid { get; set; }

        public DateTime? RequestUtcTimestampAfter { get; set; }

        public DateTime? RequestUtcTimestampBefore { get; set; }

        public string ResponseNamespace { get; set; }

        public string ResponseType { get; set; }

        public Guid? ResponseCorrelationGuid { get; set; }

        public DateTime? ResponseUtcTimestampAfter { get; set; }

        public DateTime? ResponseUtcTimestampBefore { get; set; }

        public string ExceptionContains { get; set; }

        public bool? HasException { get; set; }

        public string MessageContains { get; set; }

        public bool? HasMessage { get; set; }

        public IList<ContextCondition> ContextConditions { get; set; }

        public QueryOver<AuditEntryModel> GetQuery()
        {
            AuditEntryModel auditEntryModelAlias = null;

            var queryOver = QueryOver.Of(() => auditEntryModelAlias);

            if (!string.IsNullOrWhiteSpace(RequestNamespace))
                queryOver.Where(x => x.RequestNamespace == RequestNamespace);

            if (!string.IsNullOrWhiteSpace(RequestType))
                queryOver.Where(x => x.RequestType == RequestType);

            if (RequestCorrelationGuid.HasValue)
                queryOver.Where(x => x.RequestCorrelationGuid == RequestCorrelationGuid);

            if (RequestUtcTimestampAfter.HasValue)
                queryOver.Where(x => x.RequestUtcTimestamp >= RequestUtcTimestampAfter);

            if (RequestUtcTimestampBefore.HasValue)
                queryOver.Where(x => x.RequestUtcTimestamp <= RequestUtcTimestampBefore);

            if (!string.IsNullOrWhiteSpace(ResponseNamespace))
                queryOver.Where(x => x.ResponseNamespace == ResponseNamespace);

            if (!string.IsNullOrWhiteSpace(ResponseType))
                queryOver.Where(x => x.ResponseType == ResponseType);

            if (ResponseCorrelationGuid.HasValue)
                queryOver.Where(x => x.ResponseCorrelationGuid == ResponseCorrelationGuid);

            if (ResponseUtcTimestampAfter.HasValue)
                queryOver.Where(x => x.ResponseUtcTimestamp >= ResponseUtcTimestampAfter);

            if (ResponseUtcTimestampBefore.HasValue)
                queryOver.Where(x => x.ResponseUtcTimestamp <= ResponseUtcTimestampBefore);

            if (!string.IsNullOrWhiteSpace(ExceptionContains))
                queryOver.Where(Restrictions.On<AuditEntryModel>(r => r.Exception).IsLike(ExceptionContains, MatchMode.Anywhere));

            if (HasException.HasValue)
            {
                if (HasException.Value)
                    queryOver.Where(x => (!(x.Exception == null || x.Exception == "")));
                else
                    queryOver.Where(x => (x.Exception == null || x.Exception == ""));
            }

            if (HasMessage.HasValue)
            {
                if (HasMessage.Value)
                    queryOver.Where(x => (!(x.Message == null || x.Message == "")));
                else
                    queryOver.Where(x => (x.Message == null || x.Message == ""));
            }

            if (!string.IsNullOrWhiteSpace(MessageContains))
                queryOver.Where(Restrictions.On<AuditEntryModel>(r => r.Message).IsLike(MessageContains, MatchMode.Anywhere));

            if ((ContextConditions != null) && (ContextConditions.Where(x => !string.IsNullOrWhiteSpace(x.Key)).Count() > 0))
            {
                foreach (var contextCondition in ContextConditions.Where(x => !string.IsNullOrWhiteSpace(x.Key)))
                {
                    var localContextCondition = contextCondition;
                    var subQueryContext = QueryOver.Of<ContextEntryModel>();
                    subQueryContext.Where(x => x.AuditEntryModel.Id == auditEntryModelAlias.Id);
                    subQueryContext.Select(Projections.Id());

                    subQueryContext.Where(x => x.ContextKey == localContextCondition.Key);

                    if (!string.IsNullOrWhiteSpace(localContextCondition.ValueIs))
                        subQueryContext.Where(x => x.ContextValue == localContextCondition.ValueIs);

                    if (!string.IsNullOrWhiteSpace(localContextCondition.ValueContains))
                        subQueryContext.Where(Restrictions.On<ContextEntryModel>(r => r.ContextValue).IsLike(localContextCondition.ValueContains, MatchMode.Anywhere));

                    queryOver.WithSubquery.WhereExists(subQueryContext);
                }
            }

            return queryOver;
        }

        public class ContextCondition
        {
            public string Key { get; set; }

            public string ValueIs { get; set; }

            public string ValueContains { get; set; }
        }
    }
}
