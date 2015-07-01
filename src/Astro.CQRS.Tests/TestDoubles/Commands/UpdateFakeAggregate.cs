
namespace Astro.CQRS.Tests.TestDoubles.Commands
{
    using System;
    using Astro.CQRS;

    public class UpdateFakeAggregate : ICommand
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
    }
}
