using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Server.Models;
using FluentNHibernate.Mapping;

namespace Colombo.Clerk.Server.Mappings
{
    public class AuditEntryMap : ClassMap<AuditEntryModel>
    {
        public AuditEntryMap()
        {
            Id(x => x.Id);

            Map(x => x.RequestNamespace);
            Map(x => x.RequestType);
            Map(x => x.RequestSerialized).CustomSqlType("text");
            Map(x => x.RequestCorrelationGuid);

            Map(x => x.ResponseNamespace);
            Map(x => x.ResponseType);
            Map(x => x.ResponseSerialized).CustomSqlType("text");
            Map(x => x.ResponseCorrelationGuid);

            Map(x => x.Exception).CustomSqlType("text");
            Map(x => x.ServerMachineName);
        }
    }
}
