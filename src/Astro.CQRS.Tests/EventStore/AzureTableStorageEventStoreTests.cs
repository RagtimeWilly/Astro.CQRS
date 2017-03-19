using System;
using System.Linq;
using Astro.CQRS.EventStore;
using Astro.CQRS.Tests.TestDoubles;
using NUnit.Framework;

namespace Astro.CQRS.Tests.EventStore
{
    [TestFixture, Explicit]
    public class AzureTableStorageEventStoreTests
    {
        [Test]
        public void SaveAggregate_ToLocalStorage()
        {
            var timeProvider = new UtcTimeProvider();
            var eventStore = new AzureTableStorageEventStore("UseDevelopmentStorage=true", timeProvider);

            var id = Guid.NewGuid();

            var aggregate = FakeAggregate.Create(id) as FakeAggregate;

            aggregate.UpdateText("Some message");

            var events = eventStore.Save(aggregate);

            Assert.AreEqual(2, events.Count());

            var retrieved = eventStore.GetById<FakeAggregate>(id);

            Assert.IsNotNull(retrieved);
            Assert.AreEqual("Some message", retrieved.Text);
        }
    }
}
