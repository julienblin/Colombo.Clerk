using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class AuditEntry
    {
        public Guid Id { get; set; }

        public InnerInfo Request { get; set; }

        public InnerInfo Response { get; set; }

        public string Exception { get; set; }

        public string ServerMachineName { get; set; }

        public class InnerInfo
        {
            public string Namespace { get; set; }

            public string Type { get; set; }

            public string Serialized { get; set; }

            public Guid CorrelationGuid { get; set; }
        }
    }
}
