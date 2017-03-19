using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Astro.CQRS.Messaging;
using Moq;
using NUnit.Framework;

namespace Astro.CQRS.Tests.Messaging
{
    [TestFixture]
    public class CommandQueueSubscriberTests
    {
        [Test, Explicit]
        public void Start_SubscribesAndReceivesMessages()
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBus.ConnectionString"];
            var queueName = ConfigurationManager.AppSettings["CommandsQueueName"];

            var handlers = new List<ICommandHandler>();
            var repo = new Mock<IEventSourcedAggregateRepository>();
            var publisher = new Mock<IEventPublisher>();

            var dispatcher = new CommandDispatcher(handlers, repo.Object, publisher.Object);

            var subscriber = new CommandQueueSubscriber(connectionString, queueName, dispatcher, (ex, s) => { });

            subscriber.StartAsync();

            Thread.Sleep(60 * 1000);

            subscriber.Stop();
        }
    }
}
