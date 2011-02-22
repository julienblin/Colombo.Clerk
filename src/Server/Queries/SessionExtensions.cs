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

using NHibernate;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Queries
{
    public static class SessionExtensions
    {
        public static IQueryOver<TModelType, TModelType> GetExecQuery<TModelType>(this ISession session, IQuery<TModelType> query)
        {
            return query.GetQuery().GetExecutableQueryOver(session);
        }

        public static IQueryOver<TModelType, TModelType> GetExecQuery<TQueryType, TModelType>(this ISession session)
            where TQueryType : IQuery<TModelType>, new()
        {
            var query = new TQueryType();
            return GetExecQuery(session, query);
        }

        public static PaginationResult<TModelType> PaginateQuery<TModelType>(this ISession session, IQuery<TModelType> query, IPaginationInfo paginationInfo)
        {
            var result = new PaginationResult<TModelType>();

            var nhQuery = query.GetQuery();
            result.RowCount = nhQuery.Clone().GetExecutableQueryOver(session)
                .Select(Projections.RowCount())
                .FutureValue<int>();

            result.PaginatedQuery = nhQuery.Clone()
                .Take(paginationInfo.PerPage)
                .Skip(paginationInfo.PerPage*(paginationInfo.CurrentPage - 1));

            return result;
        }
    }

    public class PaginationResult<TModelType>
    {
        public IFutureValue<int> RowCount { get; set; }

        public QueryOver<TModelType, TModelType> PaginatedQuery { get; set; }
    }
}
