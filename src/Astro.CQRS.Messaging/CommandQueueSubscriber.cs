using System;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Astro.CQRS.Messaging
{
    public class CommandQueueSubscriber : IWorker
    {
        private readonly QueueClient _client;
        private readonly ICommandDispatcher _dispatcher;
        private readonly Action<Exception, string> _onError;
        private readonly ManualResetEvent _stopEvent;

        public CommandQueueSubscriber(string connectionString, string queueName, ICommandDispatcher dispatcher, Action<Exception, string> onError)
        {
            _dispatcher = dispatcher;
            _onError = onError;
            _client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            _stopEvent = new ManualResetEvent(false);
        }

        public async Task StartAsync()
        {
            await Task.Run(() =>
            {
                _client.OnMessage(receivedMessage =>
                {
                    try
                    {
                        var type = Type.GetType(receivedMessage.Properties["Type"].ToString());

                        var cmd = JsonConvert.DeserializeObject(receivedMessage.GetBody<string>(), type);

                        _dispatcher.Submit(Convert.ChangeType(cmd, type));
                    }
                    catch (Exception ex)
                    {
                        _onError(ex, "Error while handling command");
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
