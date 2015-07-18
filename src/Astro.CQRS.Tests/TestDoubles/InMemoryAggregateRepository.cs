
namespace Astro.CQRS.Tests.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Astro.CQRS;
    using Astro.CQRS.Exceptions;

    public class InMemoryEventSourcedRepository : EventSourcedAggregateRepositoryBase
    {
        private readonly Dictionary<Guid, List<IEvent>> eventStore = new Dictionary<Guid, List<IEvent>>();

        public override IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate)
        {
            var evtsToSave = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, evtsToSave);

            if (expectedVersion < 0)
            {
                eventStore.Add(aggregate.Id, evtsToSave);
            }
            else
            {
                var existingEvents = eventStore[aggregate.Id];
                var currentVersion = existingEvents.Count - 1;

                if (currentVersion != expectedVersion)
                {
                    throw new Exception(string.Format("Expected version {0} but the version is {1}", expectedVersion, currentVersion));
                }

                existingEvents.AddRange(evtsToSave);
            }

            return evtsToSave;
        }

        public override TResult GetById<TResult>(Guid id)
        {
            if (eventStore.ContainsKey(id))
            {
                return BuildAggregate<TResult>(eventStore[id]);
            }

            throw new AggregateNotFoundException(string.Format("Could not found aggregate of type {0} and id {1}", typeof(TResult), id));
        }
    }
}
