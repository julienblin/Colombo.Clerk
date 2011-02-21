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
using System.Diagnostics;
using System.Linq.Expressions;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using NHibernate;
using NHibernate.Criterion;

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
