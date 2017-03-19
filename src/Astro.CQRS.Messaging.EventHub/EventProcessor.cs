using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace Astro.CQRS.Messaging.EventHub
{
    public class EventProcessor : BaseDispatcher<IEventHandler>, IEventProcessor
    {
        private readonly Action<Exception, string> _onError;

        public EventProcessor(IEnumerable<IEventHandler> handlers, Action<Exception, string> onError)
            : base(typeof(IEventHandler<>), handlers)
        {
            _onError = onError;
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
                            _onError(new Exception(), $"No handler found for event: {type}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _onError(ex, "Error while handling event data");
                    }
                });
        }
    }
}
