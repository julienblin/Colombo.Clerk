using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class AuditEntryModel
    {
        public virtual Guid Id { get; set; }

        public virtual string RequestNamespace { get; set; }

        public virtual string RequestType { get; set; }

        public virtual string RequestSerialized { get; set; }

        public virtual Guid RequestCorrelationGuid { get; set; }

        public virtual string ResponseNamespace { get; set; }

        public virtual string ResponseType { get; set; }

        public virtual string ResponseSerialized { get; set; }

        public virtual Guid ResponseCorrelationGuid { get; set; }

        public virtual string Exception { get; set; }

        public virtual string ServerMachineName { get; set; }
    }
}
