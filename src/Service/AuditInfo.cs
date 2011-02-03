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
using System.Runtime.Serialization;

namespace Colombo.Clerk.Service
{
    [DataContract]
    public class AuditInfo
    {
        public AuditInfo()
        {
            Request = new InnerInfo();
            Response = new InnerInfo();
        }

        [DataMember]
        public InnerInfo Request { get; set; }

        [DataMember]
        public InnerInfo Response { get; set; }

        [DataMember]
        public string Exception { get; set; }

        private IDictionary<string, string> context;
        /// <summary>
        /// Context of the request. Garanteed to be non-null.
        /// </summary>
        public virtual IDictionary<string, string> Context
        {
            get { return context ?? (context = new Dictionary<string, string>()); }
            set { context = value; }
        }

        public override string ToString()
        {
            return string.Format("AuditInfo for {0}.{1}", Request.Namespace, Request.Type);
        }

        [DataContract]
        public class InnerInfo
        {
            [DataMember]
            public string Namespace { get; set; }

            [DataMember]
            public string Type { get; set; }

            [DataMember]
            public string Serialized { get; set; }

            [DataMember]
            public Guid CorrelationGuid { get; set; }

            [DataMember]
            public DateTime UtcTimestamp { get; set; }
        }
    }
}
