
namespace Astro.CQRS.Messaging
{
    using System;
    using System.Threading;
    using Newtonsoft.Json;
    using Serilog;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    public class EventTopicSubscriber : IWorker
    {
        SubscriptionClient _client;
        OnMessageOptions _options;
        IEventDispatcher _eventDispatcher;
        ILogger _logger;
        private readonly ManualResetEvent _stopEvent;

        public EventTopicSubscriber(string connectionString, string topicName, string subscriptionName, 
            OnMessageOptions options, IEventDispatcher eventDispatcher, ILogger logger)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
                namespaceManager.CreateSubscription(topicName, subscriptionName);

            _client = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);
            _options = options;
            _eventDispatcher = eventDispatcher;
            _logger = logger;
            _stopEvent = new ManualResetEvent(false);
        }

        public void Start()
        {
            _client.OnMessage((message) =>
            {
                try
                {
                    var type = Type.GetType(message.Properties["Type"].ToString());

                    var evt = JsonConvert.DeserializeObject(message.GetBody<string>(), type);

                    _eventDispatcher.Process(evt);
                    message.Complete();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error while handling event data");
                    message.Abandon();
                }
            }, _options);

            _stopEvent.WaitOne();
        }

        public void Stop()
        {
            _client.Close();
            _stopEvent.Set();
        }
    }
}
