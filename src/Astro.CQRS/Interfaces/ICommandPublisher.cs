
namespace Astro.CQRS
{
    using System.Threading.Tasks;

    public interface ICommandPublisher
    {
        Task PublishCommand(ICommand command);
    }
}