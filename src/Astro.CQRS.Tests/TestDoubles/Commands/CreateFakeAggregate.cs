
namespace Astro.CQRS.Tests.TestDoubles.Commands
{
    using System;
    using Astro.CQRS;

    public class CreateFakeAggregate : ICommand
    {
        public Guid Id { get; set; }
    }
}
