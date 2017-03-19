using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Serilog;

namespace Astro.CQRS.Messaging
{
    public class EventTopicPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private readonly ILogger _logger;

        public EventTopicPublisher(string connectionString, TopicDescription topic, ILogger logger)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(topic.Path))
                namespaceManager.CreateTopic(topic);

            _client = TopicClient.CreateFromConnectionString(connectionString, topic.Path);
            _logger = logger;
        }

        public async Task PublishEvent(IEvent evt)
        {
            try
            {
                await _client.SendAsync(evt.ToBrokeredMessage());
            }
            catch (Exception ex)
            {
                _logger.Error("Error while publishing event:{@evt}, ex={ex}", evt, ex.BuildExceptionInfo());
            }
        }
    }
}
