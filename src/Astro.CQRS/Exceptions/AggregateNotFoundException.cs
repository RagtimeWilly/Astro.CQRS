
namespace Astro.CQRS.Exceptions
{
    using System;

    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(Type t, Guid id)
            : this("Could not found aggregate of type " + t + " and id " + id)
        {
        }

        public AggregateNotFoundException(string message)
            : base(message)
        {
        }
    }
}
