using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using FluentNHibernate.Mapping;

namespace Colombo.Clerk.Server.Mappings
{
    public class FilterModelMap : ClassMap<FilterModel>
    {
        public FilterModelMap()
        {
            const string tableName = MapConstants.TablePrefix + "Filter";

            Table(tableName);
            Id(x => x.Id);

            Map(x => x.FilterType);
            Map(x => x.StringValue);
            Map(x => x.DateTimeValue);
            Map(x => x.BoolValue);
            Map(x => x.GuidValue);

            References(x => x.StreamModel);
        }
    }
}
