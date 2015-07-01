
namespace Astro.CQRS
{
    using System;
    using System.Collections.Generic;

    public class EventSourcedAggregateBase : IEventSourcedAggregate
    {
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        private readonly Dictionary<Type, Action<IEvent>> _eventHandlers = new Dictionary<Type, Action<IEvent>>();

        public EventSourcedAggregateBase()
        {
            this.Version = -1;
        }

        public Guid Id { get; protected set; }

        public int Version { get; private set; }

        public void RaiseEvent(IEvent evt)
        {
            this.ApplyEvent(evt);
            this._uncommitedEvents.Add(evt);
        }

        public void ApplyEvent(IEvent evt)
        {
            var eventType = evt.GetType();

            if (this._eventHandlers.ContainsKey(eventType))
            {
                this._eventHandlers[eventType](evt);
            }

            evt.Version = ++this.Version;
        }

        public IEnumerable<IEvent> UncommitedEvents()
        {
            return this._uncommitedEvents;
        }

        public void ClearUncommitedEvents()
        {
            this._uncommitedEvents.Clear();
        }

        protected void RegisterHandler<T>(Action<T> eventHandler) where T : class
        {
            this._eventHandlers.Add(typeof(T), o => eventHandler(o as T));
        }
    }
}
