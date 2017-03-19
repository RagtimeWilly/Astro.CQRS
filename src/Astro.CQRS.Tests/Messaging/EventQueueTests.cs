using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Astro.CQRS.Messaging;
using Astro.CQRS.Tests.TestDoubles.Events;
using Microsoft.ServiceBus.Messaging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Astro.CQRS.Tests.Messaging
{
    [TestFixture, Explicit]
    public class EventQueueTests
    {
        private EventQueuePublisher _evtPublisher;
        private QueueClient _client;

        [TestFixtureSetUp]
        public void Init()
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBus.ConnectionString"];
            var queueName = ConfigurationManager.AppSettings["EventsQueueName"];

            var queueDescription = new QueueDescription(queueName)
            {
                MaxSizeInMegabytes = 1024,
                DefaultMessageTimeToLive = new TimeSpan(0, 1, 0)
            };

            _evtPublisher = new EventQueuePublisher(connectionString, queueDescription, (ex, s) => { });

            _client = QueueClient.CreateFromConnectionString(connectionString, queueName);
        }

        [Test]
        public async void PublishEventToQueueTest()
        {
            var resetEvent = new ManualResetEvent(false);
            var msgReceived = false;

            var id = Guid.NewGuid();

            _client.OnMessage((msg) =>
            {
                var evt = JsonConvert.DeserializeObject(msg.GetBody<string>(), typeof(FakeAggregateCreated)) as FakeAggregateCreated;

                Assert.AreEqual(id, evt.AggregateId);

                msgReceived = true;
                resetEvent.Set();
            });

            var fakeAggregateCreated = new FakeAggregateCreated(id);

            await _evtPublisher.PublishEvent(fakeAggregateCreated);

            await Task.Delay(3000);

            Assert.IsTrue(msgReceived);
        }
    }
}
