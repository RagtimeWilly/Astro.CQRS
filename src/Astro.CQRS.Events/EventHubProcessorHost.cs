
namespace Astro.CQRS.Events
{
    using System;
    using Astro.CQRS;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Serilog;

    public class EventHubProcessorHost : IWorker
    {
        private readonly IEventProcessorFactory _eventProcessorFactory;
        private readonly string _serviceBusConnectionString;
        private readonly string _storageConnectionString;
        private readonly string _eventHubName;
        private readonly ILogger _logger;

        public EventHubProcessorHost(IEventProcessorFactory eventProcessorFactory, string serviceBusConnectionString, 
            string storageConnectionString, string eventHubName, ILogger logger)
        {
            _eventProcessorFactory = eventProcessorFactory;
            _serviceBusConnectionString = serviceBusConnectionString;
            _storageConnectionString = storageConnectionString;
            _eventHubName = eventHubName;
            _logger = logger;
        }

        public void Start()
        {
            var builder = new ServiceBusConnectionStringBuilder(_serviceBusConnectionString)
            {
                TransportType = TransportType.Amqp
            };

            var client = EventHubClient.CreateFromConnectionString(builder.ToString(), _eventHubName);

            try
            {
                var eventProcessorHost = new EventProcessorHost("singleworker",
                  client.Path, client.GetDefaultConsumerGroup().GroupName, builder.ToString(), _storageConnectionString);

                eventProcessorHost.RegisterEventProcessorFactoryAsync(_eventProcessorFactory);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while starting event processor, ex={ex}", ex.BuildExceptionInfo());
            }
        }

        public void Stop()
        {
        }
    }
}
