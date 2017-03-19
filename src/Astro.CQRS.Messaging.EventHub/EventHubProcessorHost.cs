using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;

namespace Astro.CQRS.Messaging.EventHub
{
    public class EventHubProcessorHost : IWorker
    {
        private readonly IEventProcessorFactory _eventProcessorFactory;
        private readonly string _serviceBusConnectionString;
        private readonly string _storageConnectionString;
        private readonly string _eventHubName;
        private readonly Action<Exception, string> _onError;

        public EventHubProcessorHost(IEventProcessorFactory eventProcessorFactory, string serviceBusConnectionString, 
            string storageConnectionString, string eventHubName, Action<Exception, string> onError)
        {
            _eventProcessorFactory = eventProcessorFactory;
            _serviceBusConnectionString = serviceBusConnectionString;
            _storageConnectionString = storageConnectionString;
            _eventHubName = eventHubName;
            _onError = onError;
        }

        public async Task StartAsync()
        {
            await Task.Run(() =>
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
                    _onError(ex, "Error while starting event processor");
                }
            });
        }

        public void Stop()
        {
        }
    }
}
