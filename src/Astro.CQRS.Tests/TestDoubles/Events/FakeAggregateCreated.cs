using System;

namespace Astro.CQRS.Tests.TestDoubles.Events
{
    public class FakeAggregateCreated : VersionedEvent
    {
        public FakeAggregateCreated(Guid aggregateId)
            : base(aggregateId)
        {
        }
    }
}
