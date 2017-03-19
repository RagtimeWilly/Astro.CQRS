using System.Threading.Tasks;

namespace Astro.CQRS
{
    public interface ICommandPublisher
    {
        Task PublishCommand(ICommand command);
    }
}