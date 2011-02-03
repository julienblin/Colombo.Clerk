using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using FluentNHibernate.Mapping;

namespace Colombo.Clerk.Server.Mappings
{
    public class ContextEntryModelMap : ClassMap<ContextEntryModel>
    {
        public ContextEntryModelMap()
        {
            const string tableName = MapConstants.TablePrefix + "Context";

            Table(tableName);
            Id(x => x.Id);

            Map(x => x.Key).Index(string.Format("Idx_{0}_{1}", tableName, "Key"));
            Map(x => x.Value).CustomSqlType("text");

            References(x => x.AuditEntryModel);
        }
    }
}
