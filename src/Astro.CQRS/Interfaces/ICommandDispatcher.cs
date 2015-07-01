
namespace Astro.CQRS
{
    public interface ICommandDispatcher
    {
        void Submit<TCommand>(TCommand command);
    }
}
