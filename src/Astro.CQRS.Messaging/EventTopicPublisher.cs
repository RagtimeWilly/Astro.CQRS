using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Astro.CQRS.Messaging
{
    public class EventTopicPublisher : IEventPublisher
    {
        private readonly TopicClient _client;
        private readonly Action<Exception, string> _onError;

        public EventTopicPublisher(string connectionString, TopicDescription topic, Action<Exception, string> onError)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.TopicExists(topic.Path))
                namespaceManager.CreateTopic(topic);

            _client = TopicClient.CreateFromConnectionString(connectionString, topic.Path);
            _onError = onError;
        }

        public async Task PublishEvent(IEvent evt)
        {
            try
            {
                await _client.SendAsync(evt.ToBrokeredMessage());
            }
            catch (Exception ex)
            {
                _onError(ex, "Error while publishing event");
            }
        }
    }
}
