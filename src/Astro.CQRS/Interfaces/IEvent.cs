
namespace Astro.CQRS
{
    using System;

    public interface IEvent
    {
        Guid AggregateId { get; }

        int Version { get; set; }
    }
}
