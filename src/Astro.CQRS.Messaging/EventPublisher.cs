
namespace Astro.CQRS.Messaging
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Serilog;

    public class EventPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private readonly ITimeProvider _timeProvider;
        private readonly ILogger _logger;

        public EventPublisher(string connectionString, TopicDescription topic, ITimeProvider timeProvider, ILogger logger)
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
                var message = new BrokeredMessage(JsonConvert.SerializeObject(evt));

                message.Properties["Type"] = evt.GetType().AssemblyQualifiedName;

                await _client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while publishing event:{@evt}, ex={ex}", evt, ex.BuildExceptionInfo());
            }
        }
    }
}
