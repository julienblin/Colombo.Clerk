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
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Queries;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Omu.ValueInjecter;

namespace Colombo.Clerk.Server.Handlers
{
    public class SearchAuditEntryRequestHandler : SideEffectFreeRequestHandler<SearchAuditEntryRequest, SearchAuditEntryResponse>
    {
        private readonly ISession session;

        public SearchAuditEntryRequestHandler(ISession session)
        {
            this.session = session;
        }

        protected override void Handle()
        {
            var auditEntryQuery = new AuditEntrySearchQuery();
            auditEntryQuery.InjectFrom(Request);
            if (Request.ContextConditions != null)
            {
                auditEntryQuery.ContextConditions = new List<AuditEntrySearchQuery.ContextCondition>(
                    Request.ContextConditions.Select(x =>
                    {
                        var queryContextCondition = new AuditEntrySearchQuery.ContextCondition();
                        queryContextCondition.InjectFrom(x);
                        return queryContextCondition;
                    })
                );
            }

            var query = auditEntryQuery.GetQuery();

            var rowCount = query.Clone().GetExecutableQueryOver(session)
                .Select(Projections.RowCount())
                .FutureValue<Int32>();

            var detachedResultQueryForPaging = query.Clone()
                .Take(Request.PerPage)
                .Skip(Request.PerPage * (Request.CurrentPage - 1))
                .Select(Projections.Id());

            var results = session.QueryOver<AuditEntryModel>()
                .WithSubquery.WhereProperty(x => x.Id).In(detachedResultQueryForPaging)
                .TransformUsing(Transformers.DistinctRootEntity)
                .Fetch(x => x.Context).Eager
                .Future();

            SetPaginationInfo(rowCount.Value);

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
