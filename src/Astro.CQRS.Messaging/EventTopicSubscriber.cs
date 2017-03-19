using System;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Threading.Tasks;

namespace Astro.CQRS.Messaging
{
    public class EventTopicSubscriber : IWorker
    {
        private readonly SubscriptionClient _client;
        private readonly OnMessageOptions _options;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly Action<Exception, string> _onError;
        private readonly ManualResetEvent _stopEvent;

        public EventTopicSubscriber(string connectionString, string topicName, string subscriptionName, 
            OnMessageOptions options, IEventDispatcher eventDispatcher, Action<Exception, string> onError)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionName))
                namespaceManager.CreateSubscription(topicName, subscriptionName);

            _client = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);
            _options = options;
            _eventDispatcher = eventDispatcher;
            _onError = onError;
            _stopEvent = new ManualResetEvent(false);
        }

        public async Task StartAsync()
        {
            await Task.Run(() => 
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
                        _onError(ex, "Error while handling event data");
                        message.Abandon();
                    }
                }, _options);

                _stopEvent.WaitOne();
            });
        }

        public void Stop()
        {
            _client.Close();
            _stopEvent.Set();
        }
    }
}
