using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Search.Attributes;

namespace Colombo.Clerk.Server.Models
{
    [Indexed]
    public class ContextEntryModel
    {
        [DocumentId]
        public virtual Guid Id { get; set; }

        [Field]
        public virtual string ContextKey { get; set; }

        [Field]
        public virtual string ContextValue { get; set; }

        public virtual AuditEntryModel AuditEntryModel { get; set; }
    }
}
