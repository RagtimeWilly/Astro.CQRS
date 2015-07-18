
namespace Astro.CQRS.Messaging
{
    using System;
    using System.Threading.Tasks;
    using Astro.CQRS;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Serilog;

    public class CommandPublisher : ICommandPublisher
    {
        private readonly QueueClient _client;
        private readonly ITimeProvider _timeProvider;
        private readonly ILogger _logger;

        public CommandPublisher(string connectionString, string commandQueue, ITimeProvider timeProvider, ILogger logger)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(commandQueue))
                namespaceManager.CreateQueue(commandQueue);

            _client = QueueClient.CreateFromConnectionString(connectionString, commandQueue);
            _timeProvider = timeProvider;
            _logger = logger;
        }

        public async Task PublishCommand(ICommand command)
        {
            try
            {
                var cmdType = command.GetType().AssemblyQualifiedName;
                var cmdJson = JsonConvert.SerializeObject(command);

                var envelope = new CommandEnvelope(cmdType, cmdJson, this._timeProvider.GetCurrentTime());
                var json = JsonConvert.SerializeObject(envelope);

                await _client.SendAsync(new BrokeredMessage(json));
            }
            catch (Exception ex)
            {
                _logger.Error("Error while publishing command:{@Command}, ex={ex}", command, ex.BuildExceptionInfo());
            }
        }
    }
}
