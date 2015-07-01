
namespace Astro.CQRS.Tests.TestDoubles.Events
{
    using System;
    using Astro.CQRS;

    public class FakeAggregateUpdated : VersionedEvent
    {
        public FakeAggregateUpdated(Guid aggregateId, string text)
            : base(aggregateId)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}
