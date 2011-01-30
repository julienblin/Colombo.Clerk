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
            Table(MapConstants.TablePrefix + "Context");
            Id(x => x.Id);

            Map(x => x.Key);
            Map(x => x.Value);
        }
    }
}
