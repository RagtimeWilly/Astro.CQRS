using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Astro.CQRS.Messaging
{
    public class EventQueuePublisher : QueuePublisher, IEventPublisher
    {
        public EventQueuePublisher(string connectionString, QueueDescription queueDescription, Action<Exception, string> onError)
            : base(connectionString, queueDescription, onError)
        {
        }

        public async Task PublishEvent(IEvent evt)
        {
            await Publish(evt);
        }
    }
}
