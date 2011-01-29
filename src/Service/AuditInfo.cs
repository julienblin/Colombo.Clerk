using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Colombo.Clerk.Service
{
    [DataContract]
    public class AuditInfo
    {
        public AuditInfo()
        {
            Request = new InnerInfo();
            Response = new InnerInfo();
        }

        [DataMember]
        public InnerInfo Request { get; set; }

        [DataMember]
        public InnerInfo Response { get; set; }

        [DataMember]
        public Exception Exception { get; set; }

        [DataMember]
        public string ServerMachineName { get; set; }

        private IDictionary<string, string> context;
        /// <summary>
        /// Context of the request. Garanteed to be non-null.
        /// </summary>
        public virtual IDictionary<string, string> Context
        {
            get { return context ?? (context = new Dictionary<string, string>()); }
            set { context = value; }
        }

        [DataContract]
        public class InnerInfo
        {
            [DataMember]
            public string Namespace { get; set; }

            [DataMember]
            public string Type { get; set; }

            [DataMember]
            public string Serialized { get; set; }

            [DataMember]
            public Guid CorrelationGuid { get; set; }
        }
    }
}
