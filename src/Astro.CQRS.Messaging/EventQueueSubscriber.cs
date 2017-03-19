using System.Threading.Tasks;
using Serilog;

namespace Astro.CQRS.Messaging
{
    public class EventQueueSubscriber : QueueSubscriber, IWorker
    {
        private readonly IEventDispatcher _eventDispatcher;

        public EventQueueSubscriber(string connectionString, string queueName, IEventDispatcher eventDispatcher, ILogger logger)
            : base(connectionString, queueName, logger)
        {
            _eventDispatcher = eventDispatcher;
        }

        public async Task StartAsync()
        {
            await base.StartAsync(evt => _eventDispatcher.Process(evt));
        }
    }
}
