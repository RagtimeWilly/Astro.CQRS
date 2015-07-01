
namespace Astro.CQRS.Exceptions
{
    using System;

    public class AggregateConflictException : Exception
    {
        public AggregateConflictException(Guid id, int version)
            : base(string.Format("Conflict while writing aggreate id {0} version {1}", id, version))
        {
        }
    }
}
