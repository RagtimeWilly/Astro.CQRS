using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Serilog;

namespace Astro.CQRS.Messaging
{
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
