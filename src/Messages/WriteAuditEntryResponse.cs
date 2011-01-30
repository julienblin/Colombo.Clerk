using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class WriteAuditEntryResponse : ValidatedResponse
    {
        public virtual Guid Id { get; set; }
    }
}
