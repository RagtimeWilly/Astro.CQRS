
namespace Astro.CQRS
{
    using System;

    public interface ITimeProvider
    {
        DateTime GetCurrentTime();
    }
}
