using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Serilog;

namespace Astro.CQRS.Messaging
{
    public class EventQueuePublisher : QueuePublisher, IEventPublisher
    {
        public EventQueuePublisher(string connectionString, QueueDescription queueDescription, ILogger logger)
            : base(connectionString, queueDescription, logger)
        {
        }

        public async Task PublishEvent(IEvent evt)
        {
            await Publish(evt);
        }
    }
}
