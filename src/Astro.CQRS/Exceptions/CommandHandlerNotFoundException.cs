using System;

namespace Astro.CQRS.Exceptions
{
    internal class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException(Type type)
            : base($"Command handler not found for command type: {type}")
        {
        }
    }
}
