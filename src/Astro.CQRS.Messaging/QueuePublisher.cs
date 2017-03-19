using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Serilog;
using Microsoft.ServiceBus;

namespace Astro.CQRS.Messaging
{
    public abstract class QueuePublisher
    {
        private readonly QueueClient _client;
        private readonly string _queueName;
        private readonly ILogger _logger;

        protected QueuePublisher(string connectionString, QueueDescription queueDescription, ILogger logger)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(queueDescription.Path))
                namespaceManager.CreateQueue(queueDescription);

            _client = QueueClient.CreateFromConnectionString(connectionString, queueDescription.Path);
            _queueName = queueDescription.Path;
            _logger = logger;
        }

        protected async Task Publish<T>(T msg)
        {
            try
            {
                await _client.SendAsync(msg.ToBrokeredMessage());
            }
            catch (Exception ex)
            {
                _logger.Error("Error while publishing to {queue}:{@Command}, ex={ex}", _queueName, msg, ex.BuildExceptionInfo());
            }
        }
    }
}
