﻿#region License
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
using NHibernate.Search.Attributes;

namespace Colombo.Clerk.Server.Models
{
    [Indexed]
    public class AuditEntryModel
    {
        [DocumentId]
        public virtual Guid Id { get; set; }

        [Field(Index.Tokenized)]
        public virtual string RequestNamespace { get; set; }

        [Field(Index.Tokenized)]
        public virtual string RequestType { get; set; }

        [Field(Index.Tokenized)]
        public virtual string RequestSerialized { get; set; }

        [Field(Index.NoNorms)]
        public virtual Guid RequestCorrelationGuid { get; set; }

        public virtual DateTime RequestUtcTimestamp { get; set; }

        [Field(Index.Tokenized)]
        public virtual string ResponseNamespace { get; set; }

        [Field(Index.Tokenized)]
        public virtual string ResponseType { get; set; }

        [Field(Index.Tokenized)]
        public virtual string ResponseSerialized { get; set; }

        [Field(Index.NoNorms)]
        public virtual Guid ResponseCorrelationGuid { get; set; }

        public virtual DateTime ResponseUtcTimestamp { get; set; }

        [Field(Index.Tokenized)]
        public virtual string Exception { get; set; }

        [IndexedEmbedded]
        public virtual IList<ContextEntryModel> Context { get; set; }
    }
}
