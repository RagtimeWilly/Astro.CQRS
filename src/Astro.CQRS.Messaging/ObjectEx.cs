
namespace Astro.CQRS.Messaging
{
    using System;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    internal static class ObjectEx
    {
        public static BrokeredMessage ToBrokeredMessage(this object obj)
        {
            var message = new BrokeredMessage(JsonConvert.SerializeObject(obj));

            message.MessageId = Guid.NewGuid().ToString();
            message.Properties["Type"] = obj.GetType().AssemblyQualifiedName;

            return message;
        }
    }
}
