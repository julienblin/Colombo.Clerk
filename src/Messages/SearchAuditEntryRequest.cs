#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;

namespace Colombo.Clerk.Messages
{
    public class SearchAuditEntryRequest : SideEffectFreeRequest<SearchAuditEntryResponse>
    {
        public SearchAuditEntryRequest()
        {
            PerPage = 30;
            ContextConditions = new List<ContextCondition>();
        }

        public int CurrentPage { get; set; }

        public int PerPage { get; set; }

        public string RequestNamespace { get; set; }

        public string RequestType { get; set; }

        public Guid? RequestCorrelationGuid { get; set; }

        public DateTime? RequestUtcTimestampAfter { get; set; }

        public DateTime? RequestUtcTimestampBefore { get; set; }

        public string ResponseNamespace { get; set; }

        public string ResponseType { get; set; }

        public Guid? ResponseCorrelationGuid { get; set; }

        public DateTime? ResponseUtcTimestampAfter { get; set; }

        public DateTime? ResponseUtcTimestampBefore { get; set; }

        public string ExceptionContains { get; set; }

        public IList<ContextCondition> ContextConditions { get; set; }

        public class ContextCondition
        {
            public string Key { get; set; }

            public string ValueIs { get; set; }

            public string ValueContains { get; set; }
        }
    }
}
