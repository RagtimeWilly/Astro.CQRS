using System;
using Microsoft.ServiceBus.Messaging;

namespace Astro.CQRS.Messaging.EventHub
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly Func<IEventProcessor> _makeEventProcessor;

        public EventProcessorFactory(Func<IEventProcessor> makeEventProcessor)
        {
            _makeEventProcessor = makeEventProcessor;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return _makeEventProcessor();
        }
    }
}
