using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Astro.CQRS.Messaging;
using Astro.CQRS.Tests.TestDoubles.Events;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Astro.CQRS.Tests.Messaging
{
    [TestFixture, Explicit]
    public class EventTopicTests
    {
        private EventTopicPublisher _evtPublisher;
        private SubscriptionClient _client;

        [TestFixtureSetUp]
        public void Init()
        {
            var connectionString = ConfigurationManager.AppSettings["ServiceBus.ConnectionString"];
            var topicPath = ConfigurationManager.AppSettings["TopicName"];
            var subscriptionName = "UnitTest";

            var topicDescription = new TopicDescription(topicPath)
            {
                MaxSizeInMegabytes = 1024,
                DefaultMessageTimeToLive = new TimeSpan(0, 1, 0)
            };

            _evtPublisher = new EventTopicPublisher(connectionString, topicDescription, (ex, s) => { });

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.SubscriptionExists(topicPath, subscriptionName))
                namespaceManager.CreateSubscription(topicPath, subscriptionName);

            _client = SubscriptionClient.CreateFromConnectionString(connectionString, topicPath, subscriptionName);
        }

        [Test]
        public async void PublishEventToTopicTest()
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
