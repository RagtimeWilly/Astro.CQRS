using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;

namespace Astro.CQRS.Messaging
{
    public abstract class QueueSubscriber
    {
        private readonly QueueClient _client;
        private readonly Action<Exception, string> _onError;
        private readonly string _queueName;
        private readonly ManualResetEvent _stopEvent;

        protected QueueSubscriber(string connectionString, string queueName, Action<Exception, string> onError)
        {
            _onError = onError;
            _client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            _queueName = queueName;
            _stopEvent = new ManualResetEvent(false);
        }

        protected virtual async Task StartAsync(Action<object> processMessage)
        {
            await Task.Run(() =>
            {
                _client.OnMessage(receivedMessage =>
                {
                    try
                    {
                        var type = Type.GetType(receivedMessage.Properties["Type"].ToString());

                        var cmd = JsonConvert.DeserializeObject(receivedMessage.GetBody<string>(), type);

                        processMessage(Convert.ChangeType(cmd, type));
                    }
                    catch (Exception ex)
                    {
                        _onError(ex, $"Error while handling process message from {_queueName}");
                    }
                });

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
