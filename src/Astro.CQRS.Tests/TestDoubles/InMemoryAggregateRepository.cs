using System;
using System.Collections.Generic;
using System.Linq;
using Astro.CQRS.Exceptions;

namespace Astro.CQRS.Tests.TestDoubles
{
    public class InMemoryEventSourcedRepository : EventSourcedAggregateRepositoryBase
    {
        private readonly Dictionary<Guid, List<IEvent>> _eventStore = new Dictionary<Guid, List<IEvent>>();

        public override IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate)
        {
            var evtsToSave = aggregate.UncommitedEvents().ToList();
            var expectedVersion = CalculateExpectedVersion(aggregate, evtsToSave);

            if (expectedVersion < 0)
            {
                _eventStore.Add(aggregate.Id, evtsToSave);
            }
            else
            {
                var existingEvents = _eventStore[aggregate.Id];
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
            if (_eventStore.ContainsKey(id))
            {
                return BuildAggregate<TResult>(_eventStore[id]);
            }

            throw new AggregateNotFoundException(string.Format("Could not found aggregate of type {0} and id {1}", typeof(TResult), id));
        }
    }
}
