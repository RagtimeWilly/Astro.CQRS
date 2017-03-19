using System;

namespace Astro.CQRS
{
    public interface ITimeProvider
    {
        DateTime GetCurrentTime();
    }
}
