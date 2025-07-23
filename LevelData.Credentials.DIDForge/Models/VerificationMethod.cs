using Newtonsoft.Json;
using System.Text.Json.Serialization;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace LevelData.Credentials.DIDForge.Models
{
    /// <summary>
    /// Represents an individual verification method.
    /// </summary>
    public class VerificationMethod
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonProperty("controller")]
        [JsonPropertyName("controller")]
        public string Controller { get; set; }

        [JsonProperty("publicKeyJwk", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("publicKeyJwk")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyJwk { get; set; }

        [JsonProperty("publicKeyMultibase", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("publicKeyMultibase")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string PublicKeyMultibase { get; set; }
    }
}
