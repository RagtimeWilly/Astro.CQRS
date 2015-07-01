
namespace Astro.CQRS
{
    using System;
    using Newtonsoft.Json;

    public class CommandEnvelope
    {
        public CommandEnvelope(string commandType, string commandJson, DateTime createdAt)
        {
            this.ClrCommandType = commandType;
            this.Command = commandJson;
            this.CreatedAtUtc = createdAt;
        }

        [JsonProperty] 
        public string ClrCommandType { get; private set; }

        [JsonProperty] 
        public string Command { get; private set; }

        [JsonProperty] 
        public DateTime CreatedAtUtc { get; private set; }
    }
}
