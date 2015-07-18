
namespace Astro.CQRS
{
    using System;

    public abstract class VersionedEvent : IEvent
    {
        protected VersionedEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; protected set; }

        public int Version { get; set; }
    }
}
