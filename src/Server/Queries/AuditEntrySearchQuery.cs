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
using Colombo.Clerk.Server.Models;
using NHibernate;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Queries
{
    public class AuditEntrySearchQuery
    {
        public string RequestNamespace { get; set; }

        public string RequestContextContainsKey { get; set; }

        public string RequestType { get; set; }

        public Guid RequestCorrelationGuid { get; set; }

        public string ResponseNamespace { get; set; }

        public string ResponseType { get; set; }

        public Guid ResponseCorrelationGuid { get; set; }

        public string ExceptionContains { get; set; }

        public QueryOver<AuditEntryModel> GetQuery()
        {
            var queryOver = QueryOver.Of<AuditEntryModel>();

            if (!string.IsNullOrWhiteSpace(RequestNamespace))
                queryOver.Where(x => x.RequestNamespace == RequestNamespace);

            if (!string.IsNullOrWhiteSpace(RequestType))
                queryOver.Where(x => x.RequestType == RequestType);

            if (RequestCorrelationGuid != Guid.Empty)
                queryOver.Where(x => x.RequestCorrelationGuid == RequestCorrelationGuid);

            if (!string.IsNullOrWhiteSpace(ResponseNamespace))
                queryOver.Where(x => x.ResponseNamespace == ResponseNamespace);

            if (!string.IsNullOrWhiteSpace(ResponseType))
                queryOver.Where(x => x.ResponseType == ResponseType);

            if (ResponseCorrelationGuid != Guid.Empty)
                queryOver.Where(x => x.ResponseCorrelationGuid == ResponseCorrelationGuid);

            if (!string.IsNullOrWhiteSpace(ExceptionContains))
                queryOver.Where(Restrictions.On<AuditEntryModel>(r => r.Exception).IsLike(ExceptionContains, MatchMode.Anywhere));

            return queryOver;
        }
    }
}
