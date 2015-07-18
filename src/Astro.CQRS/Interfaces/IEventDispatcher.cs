
namespace Astro.CQRS
{
    public interface IEventDispatcher
    {
        void Process<TEvent>(TEvent evt);
    }
}
