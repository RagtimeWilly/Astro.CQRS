﻿using System;
using Astro.CQRS.Tests.TestDoubles.Events;

namespace Astro.CQRS.Tests.TestDoubles
{
    public class FakeAggregate : EventSourcedAggregateBase
    {
        public FakeAggregate()
        {
            RegisterHandler<FakeAggregateCreated>(Apply);
            RegisterHandler<FakeAggregateUpdated>(Apply);
        }

        private FakeAggregate(Guid id)
            : this()
        {
            RaiseEvent(new FakeAggregateCreated(id));
        }

        public string Text { get; private set; }

        public static IEventSourcedAggregate Create(Guid id)
        {
            return new FakeAggregate(id);
        }

        public void UpdateText(string newText)
        {
            RaiseEvent(new FakeAggregateUpdated(Id, newText));
        }

        private void Apply(FakeAggregateCreated evt)
        {
            Id = evt.AggregateId;
        }

        private void Apply(FakeAggregateUpdated evt)
        {
            Text = evt.Text;
        }
    }
}
