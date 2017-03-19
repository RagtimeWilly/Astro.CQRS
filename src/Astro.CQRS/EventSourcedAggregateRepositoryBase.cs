using System;
using System.Collections.Generic;

namespace Astro.CQRS
{
    public abstract class EventSourcedAggregateRepositoryBase : IEventSourcedAggregateRepository
    {
        public abstract IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate) where TAggregate : IEventSourcedAggregate;

        public abstract TResult GetById<TResult>(Guid id) where TResult : IEventSourcedAggregate, new();

        protected int CalculateExpectedVersion<T>(IEventSourcedAggregate eventSourcedAggregate, List<T> events)
        {
            var expectedVersion = eventSourcedAggregate.Version - events.Count;
            return expectedVersion;
        }

        protected TResult BuildAggregate<TResult>(IEnumerable<IEvent> events) where TResult : IEventSourcedAggregate, new()
        {
            var result = new TResult();

            foreach (var evt in events)
            {
                result.ApplyEvent(evt);
            }

            return result;
        }
    }
}
