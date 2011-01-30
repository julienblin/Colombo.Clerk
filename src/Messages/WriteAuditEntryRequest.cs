using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class WriteAuditEntryRequest : Request<WriteAuditEntryResponse>
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string RequestNamespace { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string RequestType { get; set; }

        public string RequestSerialized { get; set; }

        public Guid RequestCorrelationGuid { get; set; }

        public string ResponseNamespace { get; set; }

        public string ResponseType { get; set; }

        public string ResponseSerialized { get; set; }

        public virtual Guid ResponseCorrelationGuid { get; set; }

        public string Exception { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string ServerMachineName { get; set; }
    }
}
