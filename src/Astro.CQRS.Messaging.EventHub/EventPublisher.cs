﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Serilog;

namespace Astro.CQRS.Messaging.EventHub
{
    public class EventPublisher : IEventPublisher
    {
        private readonly string _partitionKey;
        private readonly ILogger _logger;
        private readonly EventHubClient _client;

        public EventPublisher(string connectionString, string eventHubName, string partitionKey, ILogger logger)
        {
            var builder = new ServiceBusConnectionStringBuilder(connectionString) { TransportType = TransportType.Amqp };

            _partitionKey = partitionKey;
            _client = EventHubClient.CreateFromConnectionString(builder.ToString(), eventHubName);
            _logger = logger;
        }

        public async Task PublishEvent(IEvent evt)
        {
            try
            {
                var json = JsonConvert.SerializeObject(evt);
                var data = new EventData(Encoding.Unicode.GetBytes(json))
                {
                    PartitionKey = _partitionKey
                };

                data.Properties.Add("Type", evt.GetType().AssemblyQualifiedName);
                
                await _client.SendAsync(data);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while publishing event:{@Event}, ex={ex}", evt, ex.BuildExceptionInfo());
            }
        }
    }
}
