using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class FilterModel
    {
        public virtual Guid Id { get; set; }

        public virtual FilterType FilterType { get; set; }

        public virtual string StringValue { get; set; }

        public virtual DateTime? DateTimeValue { get; set; }

        public virtual bool? BoolValue { get; set; }

        public virtual Guid? GuidValue { get; set; }

        public virtual StreamModel StreamModel { get; set; }
    }

    public enum FilterType
    {
        @String,
        @DateTime,
        @Bool,
        @Guid
    }
}
