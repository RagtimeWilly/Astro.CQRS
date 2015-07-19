
namespace Astro.CQRS.Messaging
{
    using System.Threading.Tasks;
    using Astro.CQRS;
    using Microsoft.ServiceBus.Messaging;
    using Serilog;

    public class CommandPublisher : QueuePublisher, ICommandPublisher
    {
        public CommandPublisher(string connectionString, QueueDescription queueDescription, ILogger logger)
            : base(connectionString, queueDescription, logger)
        {
        }

        public async Task PublishCommand(ICommand command)
        {
            await Publish(command);
        }
    }
}
