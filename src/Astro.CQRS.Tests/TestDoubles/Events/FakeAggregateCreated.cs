
namespace Astro.CQRS.Tests.TestDoubles.Events
{
    using System;
    using Astro.CQRS;

    public class FakeAggregateCreated : VersionedEvent
    {
        public FakeAggregateCreated(Guid aggregateId)
            : base(aggregateId)
        {
        }
    }
}
