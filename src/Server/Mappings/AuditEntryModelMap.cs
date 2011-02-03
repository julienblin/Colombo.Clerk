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

using Colombo.Clerk.Server.Models;
using FluentNHibernate.Mapping;

namespace Colombo.Clerk.Server.Mappings
{
    public class AuditEntryModelMap : ClassMap<AuditEntryModel>
    {
        public AuditEntryModelMap()
        {
            const string tableName = MapConstants.TablePrefix + "AuditEntry";

            Table(tableName);
            Id(x => x.Id);

            Map(x => x.RequestNamespace).Index(string.Format("Idx_{0}_{1}", tableName, "RequestNamespace"));
            Map(x => x.RequestType).Index(string.Format("Idx_{0}_{1}", tableName, "RequestType"));
            Map(x => x.RequestSerialized).CustomSqlType("text");
            Map(x => x.RequestCorrelationGuid);
            Map(x => x.RequestUtcTimestamp).Index(string.Format("Idx_{0}_{1}", tableName, "RequestUtcTimestamp"));

            Map(x => x.ResponseNamespace).Index(string.Format("Idx_{0}_{1}", tableName, "ResponseNamespace"));
            Map(x => x.ResponseType).Index(string.Format("Idx_{0}_{1}", tableName, "ResponseType"));
            Map(x => x.ResponseSerialized).CustomSqlType("text");
            Map(x => x.ResponseCorrelationGuid);
            Map(x => x.ResponseUtcTimestamp).Index(string.Format("Idx_{0}_{1}", tableName, "ResponseUtcTimestamp"));

            Map(x => x.Exception).CustomSqlType("text");

            HasMany(x => x.Context).Cascade.AllDeleteOrphan();
        }
    }
}
