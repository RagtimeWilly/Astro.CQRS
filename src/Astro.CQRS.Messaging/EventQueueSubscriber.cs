using System;
using System.Threading.Tasks;

namespace Astro.CQRS.Messaging
{
    public class EventQueueSubscriber : QueueSubscriber, IWorker
    {
        private readonly IEventDispatcher _eventDispatcher;

        public EventQueueSubscriber(string connectionString, string queueName, IEventDispatcher eventDispatcher, Action<Exception, string> onError)
            : base(connectionString, queueName, onError)
        {
            _eventDispatcher = eventDispatcher;
        }

        public async Task StartAsync()
        {
            await base.StartAsync(evt => _eventDispatcher.Process(evt));
        }
    }
}
