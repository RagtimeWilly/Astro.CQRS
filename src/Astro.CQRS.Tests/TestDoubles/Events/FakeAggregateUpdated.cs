using System;

namespace Astro.CQRS.Tests.TestDoubles.Events
{
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
