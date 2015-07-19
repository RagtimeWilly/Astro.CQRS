
namespace Astro.CQRS.Tests
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading;
    using Astro.CQRS.Messaging;
    using Moq;
    using NUnit.Framework;
    using Serilog;

    [TestFixture]
    public class CommandQueueSubscriberTests
    {
        [Test, Explicit]
        public void Start_SubscribesAndReceivesMessages()
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBus.ConnectionString"];
            var queueName = ConfigurationManager.AppSettings["QueueName"];

            var handlers = new List<ICommandHandler>();
            var repo = new Mock<IEventSourcedAggregateRepository>();
            var publisher = new Mock<IEventPublisher>();
            var logger = new Mock<ILogger>();

            var dispatcher = new CommandDispatcher(handlers, repo.Object, publisher.Object);

            var subscriber = new CommandQueueSubscriber(connectionString, queueName, dispatcher, logger.Object);

            subscriber.StartAsync();

            Thread.Sleep(60 * 60 * 1000);
        }
    }
}
