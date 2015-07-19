
namespace Astro.CQRS.Messaging
{
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Serilog;

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
