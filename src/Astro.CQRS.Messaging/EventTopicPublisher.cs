
namespace Astro.CQRS.Messaging
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Serilog;

    public class EventTopicPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private readonly ITimeProvider _timeProvider;
        private readonly ILogger _logger;

        public EventTopicPublisher(string connectionString, TopicDescription topic, ITimeProvider timeProvider, ILogger logger)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(topic.Path))
                namespaceManager.CreateTopic(topic);

            _client = TopicClient.CreateFromConnectionString(connectionString, topic.Path);
            _timeProvider = timeProvider;
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
