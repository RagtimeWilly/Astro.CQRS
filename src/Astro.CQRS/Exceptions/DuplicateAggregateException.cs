using System;

namespace Astro.CQRS.Exceptions
{
    public abstract class DuplicateAggregateException : Exception
    {
        protected DuplicateAggregateException(Guid id)
            : base(CreateMessage(id))
        {
        }

        private static string CreateMessage(Guid id)
        {
            return $"Aggregate already exists with id {id}";
        }
    }
}
