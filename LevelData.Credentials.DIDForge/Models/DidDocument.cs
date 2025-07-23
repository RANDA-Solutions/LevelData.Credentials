using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace LevelData.Credentials.DIDForge.Models
{
    /// <summary>
    /// Represents a DID Document following the W3C DID Core syntax.
    /// For details, see: https://www.w3.org/TR/did-1.0/#did-syntax
    /// </summary>
    public class DidDocument
    {
        [JsonProperty("@context")]
        [JsonPropertyName("@context")]
        public List<string> Context { get; set; } = new List<string> { "https://www.w3.org/ns/did/v1" };

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("alsoKnownAs", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("alsoKnownAs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> AlsoKnownAs { get; set; }

        [JsonProperty("controller", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("controller")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object Controller { get; set; }

        [JsonProperty("verificationMethod", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("verificationMethod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<VerificationMethod> VerificationMethod { get; set; }

        [JsonProperty("authentication", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("authentication")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> Authentication { get; set; }

        [JsonProperty("assertionMethod", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("assertionMethod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> AssertionMethod { get; set; }

        [JsonProperty("keyAgreement", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("keyAgreement")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> KeyAgreement { get; set; }

        [JsonProperty("capabilityInvocation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("capabilityInvocation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> CapabilityInvocation { get; set; }

        [JsonProperty("capabilityDelegation", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("capabilityDelegation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> CapabilityDelegation { get; set; }

        [JsonProperty("service", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("service")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Service> Service { get; set; }

        public void AddVerificationMethod(VerificationMethod verificationMethod, bool allowAuthentication = true, bool allowAssertion = true)
        {
            if (VerificationMethod == null)
            {
                VerificationMethod = new List<VerificationMethod>();
            }
            VerificationMethod.Add(verificationMethod);

            if (allowAuthentication)
            {
                AddAuthentication(verificationMethod.Id);
            }

            if (allowAssertion)
            {
                AddAssertionMethod(verificationMethod.Id);
            }
        }

        public void AddAssertionMethod(VerificationMethod verificationMethod)
        {
            if (AssertionMethod == null)
            {
                AssertionMethod = new List<object>();
            }
            AssertionMethod.Add(verificationMethod.Id);
        }

        public void AddAssertionMethod(string verificationMethodId)
        {
            if (AssertionMethod == null)
            {
                AssertionMethod = new List<object>();
            }
            AssertionMethod.Add(verificationMethodId);
        }

        public void AddAuthentication(VerificationMethod verificationMethod)
        {
            if (Authentication == null)
            {
                Authentication = new List<object>();
            }
            Authentication.Add(verificationMethod.Id);
        }

        public void AddAuthentication(string verificationMethodId)
        {
            if (Authentication == null)
            {
                Authentication = new List<object>();
            }
            Authentication.Add(verificationMethodId);
        }
    }
}
