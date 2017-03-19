
namespace Astro.CQRS
{
    using System;
    using System.Collections.Generic;

    public class EventDispatcher : BaseDispatcher<IEventHandler>, IEventDispatcher
    {
        public EventDispatcher(IEnumerable<IEventHandler> eventHandlers)
            : base(typeof(IEventHandler<>), eventHandlers)
        {
        }

        public void Process<TEvent>(TEvent evt)
        {
            if (Handlers.ContainsKey(evt.GetType()))
            {
                dynamic handler = Handlers[evt.GetType()];

                handler.HandleEvent((dynamic)evt);
            }
            else
            {
                throw new Exception("No event handler found for event:" + evt.GetType());
            }
        }
    }
}
