using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class ContextEntryModel
    {
        public virtual Guid Id { get; set; }

        public virtual string Key { get; set; }

        public virtual string Value { get; set; }
    }
}
