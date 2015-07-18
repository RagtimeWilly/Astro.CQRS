
namespace Astro.CQRS.Messaging
{
    using System;
    using System.Threading;
    using Astro.CQRS;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Serilog;

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

        public void Start()
        {
            _logger.Information("Starting processing of commands");

            _client.OnMessage(receivedMessage =>
            {
                try
                {
                    var json = receivedMessage.GetBody<string>();

                    var envelope = JsonConvert.DeserializeObject<CommandEnvelope>(json);
                    var cmdType = Type.GetType(envelope.ClrCommandType);

                    var cmd = JsonConvert.DeserializeObject(envelope.Command, cmdType);

                    _dispatcher.Submit(Convert.ChangeType(cmd, cmdType));
                }
                catch (Exception ex)
                {
                    _logger.Error("Error while handling command, ex={ex}", ex.BuildExceptionInfo());
                }
            });

            _stopEvent.WaitOne();
        }

        public void Stop()
        {
            _client.Close();
            _stopEvent.Set();
        }
    }
}
