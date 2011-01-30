using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using FluentNHibernate.Mapping;

namespace Colombo.Clerk.Server.Mappings
{
    public class AuditEntryMap : ClassMap<AuditEntry>
    {
        public AuditEntryMap()
        {
            Id(x => x.Id);

            Map(x => x.RequestNamespace);
            Map(x => x.RequestType);
            Map(x => x.RequestSerialized);
            Map(x => x.RequestCorrelationGuid);
        }
    }
}
