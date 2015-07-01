
namespace Astro.CQRS
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler 
        where TCommand : ICommand
    {
        IEventSourcedAggregate Execute(TCommand cmd);
    }
}
