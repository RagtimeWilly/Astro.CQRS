using System;
using System.Collections.Generic;
using System.Linq;
using Astro.CQRS.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Astro.CQRS.EventStore
{
    public class AzureTableStorageEventStore : EventSourcedAggregateRepositoryBase
    {
        private readonly CloudTableClient _tableClient;
        private readonly ITimeProvider _timeProvider;

        public AzureTableStorageEventStore(string storageConnectionString, ITimeProvider timeProvider)
        {
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            _tableClient = storageAccount.CreateCloudTableClient();
            _timeProvider = timeProvider;
        }

        public override IEnumerable<IEvent> Save<TAggregate>(TAggregate aggregate)
        {
            var tableName = aggregate.GetType().Name;

            var table = _tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();

            var events = aggregate.UncommitedEvents().ToList();

            foreach (var evt in aggregate.UncommitedEvents())
            {
                var formattedVersion = evt.Version.ToString("D10");

                var tableEntity = new EventAzureTableEntity
                    {
                        AggregateId = aggregate.Id.ToString(),
                        PartitionKey = aggregate.Id.ToString(),
                        RowKey = formattedVersion,
                        CreatedAt = _timeProvider.GetCurrentTime().ToString("o"),
                        ClrType = evt.GetType().AssemblyQualifiedName,
                        TypeName = evt.GetType().Name,
                        Payload = JsonConvert.SerializeObject(evt)
                    };

                var insertOperation = TableOperation.Insert(tableEntity);

                try
                {
                    table.Execute(insertOperation);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("409"))
                        throw new AggregateConflictException(aggregate.Id, evt.Version);

                    throw;
                }
            }

            return events;
        }

        public override TResult GetById<TResult>(Guid id)
        {
            try
            {
                var table = _tableClient.GetTableReference(typeof(TResult).Name);

                var query = new TableQuery<EventAzureTableEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id.ToString()));

                var result = table.ExecuteQuery(query).ToList();

                if (!result.Any())
                    throw new AggregateNotFoundException(typeof(TResult), id);

                var deserializedEvents = result.Select(DeserializeAzureEntity);

                return BuildAggregate<TResult>(deserializedEvents);
            }
            catch (StorageException ex)
            {
                if (ex.Message.Contains("404"))
                    throw new AggregateNotFoundException(typeof(TResult), id);

                throw;
            }
        }

        private static IEvent DeserializeAzureEntity(EventAzureTableEntity tableEntity)
        {
            return JsonConvert.DeserializeObject(tableEntity.Payload, Type.GetType(tableEntity.ClrType)) as IEvent;
        }
    }
}
