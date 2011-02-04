using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class ContextEntryModel
    {
        public virtual Guid Id { get; set; }

        public virtual string ContextKey { get; set; }

        public virtual string ContextValue { get; set; }

        public virtual AuditEntryModel AuditEntryModel { get; set; }
    }
}
