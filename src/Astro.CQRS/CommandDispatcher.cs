
namespace Astro.CQRS
{
    using System;
    using System.Collections.Generic;

    public class CommandDispatcher : BaseDispatcher<ICommandHandler>, ICommandDispatcher
    {
        private readonly IEventSourcedAggregateRepository _aggregateRepository;
        private readonly IEventPublisher _eventPublisher;

        public CommandDispatcher(
            IEnumerable<ICommandHandler> commandHandlers, 
            IEventSourcedAggregateRepository aggregateRepository, 
            IEventPublisher eventPublisher)
            : base(typeof(ICommandHandler<>), commandHandlers)
        {
            _aggregateRepository = aggregateRepository;
            _eventPublisher = eventPublisher;
        }

        public void Submit<TCommand>(TCommand command)
        {
            if (_handlers.ContainsKey(command.GetType()))
            {
                dynamic handler = _handlers[command.GetType()];

                var aggregate = handler.Execute((dynamic)command);
               
                var newEvents = aggregate.UncommitedEvents();

                _aggregateRepository.Save(aggregate);

                foreach (var evt in newEvents)
                    _eventPublisher.PublishEvent(evt);
            }
            else
            {
                throw new Exception("No command handler found for command:" + command.GetType());
            }
        }
    }
}
