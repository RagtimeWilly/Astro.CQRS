using System;

namespace Astro.CQRS
{
    public interface IEvent
    {
        Guid AggregateId { get; }

        int Version { get; set; }
    }
}
