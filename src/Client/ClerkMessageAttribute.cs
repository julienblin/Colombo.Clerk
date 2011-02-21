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
using System.IO;
using System.Resources;
using NVelocity;
using NVelocity.App;

namespace Colombo.Clerk.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClerkMessageAttribute : Attribute
    {
        static ClerkMessageAttribute()
        {
            Velocity.Init();
        }

        public ClerkMessageAttribute(string message)
        {
            Message = message;
        }

        public ClerkMessageAttribute(string messageResourceName, Type messageResourceType)
        {
            MessageResourceName = messageResourceName;
            MessageResourceType = messageResourceType;
        }

        public string Message { get; private set; }

        public string MessageResourceName { get; private set; }

        public Type MessageResourceType { get; private set; }

        public string GetRealMessage(BaseRequest request, Response response)
        {
            var baseMessage = GetBaseMessage();
            if (baseMessage == null)
                return null;

            var context = new VelocityContext();
            context.Put("request", request);
            context.Put("response", response);

            var result = new StringWriter();

            Velocity.Evaluate(context, result, "", baseMessage);
            return result.ToString();
        }

        public string GetBaseMessage()
        {
            if (MessageResourceType == null)
                return Message;

            return new ResourceManager(MessageResourceType).GetString(MessageResourceName);
        }
    }
}
