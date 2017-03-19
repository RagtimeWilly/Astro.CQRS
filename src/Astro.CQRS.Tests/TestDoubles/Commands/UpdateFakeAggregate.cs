using System;

namespace Astro.CQRS.Tests.TestDoubles.Commands
{
    public class UpdateFakeAggregate : ICommand
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}
