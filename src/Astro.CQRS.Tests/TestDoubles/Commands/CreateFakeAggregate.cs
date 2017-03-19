using System;

namespace Astro.CQRS.Tests.TestDoubles.Commands
{
    public class CreateFakeAggregate : ICommand
    {
        public Guid Id { get; set; }
    }
}
