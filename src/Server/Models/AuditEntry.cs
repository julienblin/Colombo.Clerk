using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class AuditEntry
    {
        public virtual Guid Id { get; set; }

        public virtual string RequestNamespace { get; set; }

        public virtual string RequestType { get; set; }

        public virtual string RequestSerialized { get; set; }

        public virtual Guid RequestCorrelationGuid { get; set; }
    }
}
