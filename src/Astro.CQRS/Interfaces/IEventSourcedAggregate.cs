using System;
using System.Collections.Generic;

namespace Astro.CQRS
{
    public interface IEventSourcedAggregate
    {
        Guid Id { get; }

        int Version { get; }

        IEnumerable<IEvent> UncommitedEvents();

        void ClearUncommitedEvents();

        void ApplyEvent(IEvent @event);
    }
}
