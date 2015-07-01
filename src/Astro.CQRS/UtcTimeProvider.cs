
namespace Astro.CQRS
{
    using System;

    public class UtcTimeProvider : ITimeProvider
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }
    }
}
