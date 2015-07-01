﻿
namespace Astro.CQRS.EventStore
{
    using Microsoft.WindowsAzure.Storage.Table;

    public sealed class EventAzureTableEntity : TableEntity
    {
        public string AggregateId { get; set; }

        public string CorrelationId { get; set; }

        public string Payload { get; set; }

        public string CreatedAt { get; set; }

        public string TypeName { get; set; }

        public string ClrType { get; set; }
    }
}
