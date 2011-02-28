using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class StreamModel
    {
        public virtual Guid Id { get; set; }

        public virtual string Name { get; set; }

        public virtual IList<FilterModel> Filters { get; set; }
    }
}
