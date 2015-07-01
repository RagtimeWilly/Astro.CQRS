
namespace Astro.CQRS.Tests.TestDoubles
{
    using Astro.CQRS;
    using Astro.CQRS.Tests.TestDoubles.Commands;

    public class FakeCommandHandler : ICommandHandler<CreateFakeAggregate>, 
                                      ICommandHandler<UpdateFakeAggregate>
    {
        private readonly IEventSourcedAggregateRepository _repository;

        public FakeCommandHandler(IEventSourcedAggregateRepository repository)
        {
            _repository = repository;
        }

        public IEventSourcedAggregate Execute(CreateFakeAggregate cmd)
        {
            return FakeAggregate.Create(cmd.Id);
        }

        public IEventSourcedAggregate Execute(UpdateFakeAggregate cmd)
        {
            var aggregate = _repository.GetById<FakeAggregate>(cmd.Id);

            aggregate.UpdateText(cmd.Text);

            return aggregate;
        }
    }
}
