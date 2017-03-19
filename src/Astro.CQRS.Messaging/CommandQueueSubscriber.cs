using System;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Serilog;
using System.Threading.Tasks;

namespace Astro.CQRS.Messaging
{
    public class CommandQueueSubscriber : IWorker
    {
        private readonly QueueClient _client;
        private readonly ICommandDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly ManualResetEvent _stopEvent;

        public CommandQueueSubscriber(string connectionString, string queueName, ICommandDispatcher dispatcher, ILogger logger)
        {
            _dispatcher = dispatcher;
            _logger = logger;
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
                        _logger.Error("Error while handling command, ex={ex}", ex.BuildExceptionInfo());
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
