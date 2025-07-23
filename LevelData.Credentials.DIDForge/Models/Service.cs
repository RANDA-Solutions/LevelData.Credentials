using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LevelData.Credentials.DIDForge.Models
{
    /// <summary>
    /// Represents a service endpoint map.
    /// </summary>
    public class Service
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonProperty("serviceEndpoint")]
        [JsonPropertyName("serviceEndpoint")]
        public object ServiceEndpoint { get; set; }
    }

}
