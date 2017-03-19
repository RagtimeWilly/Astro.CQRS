using System.Threading.Tasks;

namespace Astro.CQRS
{
    public interface IEventPublisher
    {
        Task PublishEvent(IEvent evt);
    }
}