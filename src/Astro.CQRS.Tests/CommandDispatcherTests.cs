
namespace Astro.CQRS.Tests
{
    using System;
    using Astro.CQRS;
    using Astro.CQRS.Tests.TestDoubles;
    using Astro.CQRS.Tests.TestDoubles.Commands;
    using Astro.CQRS.Tests.TestDoubles.Events;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CommandDispatcherTests
    {
        [Test]
        public void Submit_CallsHandlerSavesAndPublishesEvents()
        {
            var repo = new InMemoryEventSourcedRepository();
            var handler = new FakeCommandHandler(repo);
            var publisher = new Mock<IEventPublisher>();

            var dispatcher = new CommandDispatcher(new[] { handler }, repo, publisher.Object);

            var id = Guid.NewGuid();

            var updatedText = Guid.NewGuid().ToString();

            dispatcher.Submit(new CreateFakeAggregate { Id = id });
            dispatcher.Submit(new UpdateFakeAggregate { Id = id, Text = updatedText });

            var aggreate = repo.GetById<FakeAggregate>(id);

            Assert.AreEqual(updatedText, aggreate.Text);

            publisher.Verify(f => f.PublishEvent(It.Is<IEvent>(e => e.GetType() == typeof(FakeAggregateCreated))), Times.Once);
            publisher.Verify(f => f.PublishEvent(It.Is<IEvent>(e => e.GetType() == typeof(FakeAggregateUpdated))), Times.Once);
        }
    }
}
