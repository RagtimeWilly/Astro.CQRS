
namespace Astro.CQRS.Exceptions
{
    using System;

    public abstract class DuplicateAggregateException : Exception
    {
        protected DuplicateAggregateException(Guid id)
            : base(CreateMessage(id))
        {
        }

        private static string CreateMessage(Guid id)
        {
            return string.Format("Aggregate already exists with id {0}", id);
        }
    }
}
