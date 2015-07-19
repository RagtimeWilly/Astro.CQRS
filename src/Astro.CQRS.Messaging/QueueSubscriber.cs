
namespace Astro.CQRS.Messaging
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Microsoft.ServiceBus.Messaging;
    using Serilog;

    public abstract class QueueSubscriber
    {
        private readonly QueueClient _client;
        private readonly ICommandDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly string _queueName;
        private readonly ManualResetEvent _stopEvent;

        public QueueSubscriber(string connectionString, string queueName, ILogger logger)
        {
            _logger = logger;
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
                        _logger.Error(ex, "Error while handling process message from {queue}, ex={ex}", _queueName);
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
