using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Server.Models
{
    public class FilterModel
    {
        public virtual Guid Id { get; set; }

        public virtual string FilterName { get; set; }

        public virtual string StringValue { get; set; }

        public virtual DateTime? DateTimeValue { get; set; }

        public virtual bool? BoolValue { get; set; }

        public virtual Guid? GuidValue { get; set; }

        public virtual StreamModel StreamModel { get; set; }

        public virtual void SetValue(Type filterType, object value)
        {
            if(filterType == typeof(string))
                StringValue = (string)value;

            if(filterType == typeof(DateTime))
                DateTimeValue = (DateTime) value;

            if(filterType == typeof(bool))
                BoolValue = (bool) value;

            if(filterType == typeof(Guid))
                GuidValue = (Guid) value;
        }
    }
}
