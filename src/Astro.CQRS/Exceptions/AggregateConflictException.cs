using System;

namespace Astro.CQRS.Exceptions
{
    public class AggregateConflictException : Exception
    {
        public AggregateConflictException(Guid id, int version)
            : base($"Conflict while writing aggreate id {id} version {version}")
        {
        }
    }
}
