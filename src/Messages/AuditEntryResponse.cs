using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class AuditEntryResponse : Response
    {
        public virtual bool Found { get; set; }

        public virtual Guid Id { get; set; }

        public virtual InnerInfo Request { get; set; }

        public virtual InnerInfo Response { get; set; }

        public virtual string Exception { get; set; }

        public virtual string ServerMachineName { get; set; }

        public class InnerInfo
        {
            public string Namespace { get; set; }

            public string Type { get; set; }

            public string Serialized { get; set; }

            public Guid CorrelationGuid { get; set; }
        }
    }
}
