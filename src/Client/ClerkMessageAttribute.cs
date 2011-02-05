using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
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
