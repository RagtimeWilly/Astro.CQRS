using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Microsoft.ServiceBus;

namespace Astro.CQRS.Messaging
{
    public abstract class QueuePublisher
    {
        private readonly QueueClient _client;
        private readonly string _queueName;
        private readonly Action<Exception, string> _onError;

        protected QueuePublisher(string connectionString, QueueDescription queueDescription, Action<Exception, string> onError)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(queueDescription.Path))
                namespaceManager.CreateQueue(queueDescription);

            _client = QueueClient.CreateFromConnectionString(connectionString, queueDescription.Path);
            _queueName = queueDescription.Path;
            _onError = onError;
        }

        protected async Task Publish<T>(T msg)
        {
            try
            {
                await _client.SendAsync(msg.ToBrokeredMessage());
            }
            catch (Exception ex)
            {
                _onError(ex, $"Error while publishing to {_queueName}");
            }
        }
    }
}
