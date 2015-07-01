
namespace Astro.CQRS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class BaseDispatcher<T>
    {
        protected readonly Type _genericHandler;
        protected readonly Dictionary<Type, T> _handlers;

        protected BaseDispatcher(Type genericHandler, IEnumerable<T> handlers)
        {
            _genericHandler = genericHandler;

            _handlers = new Dictionary<Type, T>();

            foreach (var handler in handlers)
                Register(handler);
        }

        protected void Register(T handler)
        {
            var cmdTypes = handler.GetType()
                                  .GetInterfaces()
                                  .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == _genericHandler)
                                  .Select(i => i.GetGenericArguments()[0])
                                  .ToList();

            if (_handlers.Keys.Any(cmdTypes.Contains))
                throw new ArgumentException("Only one handler per type is allowed");

            foreach (var cmdType in cmdTypes)
                _handlers.Add(cmdType, handler);
        }
    }
}
