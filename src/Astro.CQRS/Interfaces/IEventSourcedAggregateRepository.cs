using System;
using System.Collections.Generic;

namespace Astro.CQRS
{
    public interface IEventSourcedAggregateRepository
    {
        IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IEventSourcedAggregate;

        TResult GetById<TResult>(Guid id) where TResult : IEventSourcedAggregate, new();
    }
}
