using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Astro.CQRS.Messaging
{
    public class CommandPublisher : QueuePublisher, ICommandPublisher
    {
        public CommandPublisher(string connectionString, QueueDescription queueDescription, Action<Exception, string> onError)
            : base(connectionString, queueDescription, onError)
        {
        }

        public async Task PublishCommand(ICommand command)
        {
            await Publish(command);
        }
    }
}
