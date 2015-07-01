
namespace Astro.CQRS
{
    using System.Threading.Tasks;

    public interface IEventPublisher
    {
        Task PublishEvent(IEvent evt);
    }
}