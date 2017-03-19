
namespace Astro.CQRS.Messaging.EventHub
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Astro.CQRS;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Serilog;

    public class EventProcessor : BaseDispatcher<IEventHandler>, IEventProcessor
    {
        private readonly ILogger _logger;

        public EventProcessor(IEnumerable<IEventHandler> handlers, ILogger logger)
            : base(typeof(IEventHandler<>), handlers)
        {
            _logger = logger;
        }

        public Task OpenAsync(PartitionContext context)
        {
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            foreach (var eventData in events)
            {
                ProcessEvent(eventData);
            }

            await context.CheckpointAsync();
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        private void ProcessEvent(EventData eventData)
        {
            Task.Run(() =>
                {
                    try
                    {
                        var type = Type.GetType(eventData.Properties["Type"].ToString());
                        var bytes = Encoding.Unicode.GetString(eventData.GetBytes());

                        var evt = JsonConvert.DeserializeObject(bytes, type);

                        if (Handlers.ContainsKey(evt.GetType()))
                        {
                            dynamic handler = Handlers[evt.GetType()];

                            handler.HandleEvent((dynamic)evt);
                        }
                        else
                        {
                            _logger.Warning("No handler found for event: {event}", evt);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error while handling event data, ex={ex}", ex.BuildExceptionInfo());
                    }
                });
        }
    }
}
