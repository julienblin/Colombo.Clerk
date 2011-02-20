using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class GetDistinctValuesRequest : SideEffectFreeRequest<GetDistinctValuesResponse>
    {
        public GetDistinctValueType ValueType { get; set; }
    }

    public enum GetDistinctValueType
    {
        RequestNamespace,
        RequestType,
        ResponseNamespace,
        ResponseType,
        ContextKey
    }
}
