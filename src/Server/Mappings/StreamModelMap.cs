using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using FluentNHibernate.Mapping;

namespace Colombo.Clerk.Server.Mappings
{
    public class StreamModelMap : ClassMap<StreamModel>
    {
        public StreamModelMap()
        {
            const string tableName = MapConstants.TablePrefix + "Stream";

            Table(tableName);
            Id(x => x.Id);

            Map(x => x.Name).Unique();

            HasMany(x => x.Filters).Cascade.AllDeleteOrphan();
        }
    }
}
