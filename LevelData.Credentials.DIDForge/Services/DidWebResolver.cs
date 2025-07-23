using LevelData.Credentials.DIDForge.Abstractions;
using LevelData.Credentials.DIDForge.Models;
using System.Text.Json;

namespace LevelData.Credentials.DIDForge.Services
{
    /// <summary>
    /// Resolves did:web DID documents by mapping the DID to the appropriate URL.
    /// For example, the DID
    ///   did:web:randaocpservice-test.azurewebsites.net:issuers:issuerId:requestId
    /// is mapped to
    ///   https://randaocpservice-test.azurewebsites.net/issuers/issuerId/requestId/.well-known/did.json
    /// </summary>
    public class DidWebResolver : IDidResolver
    {
        private readonly HttpClient _httpClient;

        public DidWebResolver(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DidDocument> ResolveDidDocumentAsync(string did)
        {
            if (!did.StartsWith("did:web:", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid DID format; expected did:web", nameof(did));
            }

            // Remove fragment if present (anything after '#')
            var didWithoutFragment = did.Split('#')[0];

            // Remove the did:web prefix.
            var didWithoutPrefix = didWithoutFragment.Substring("did:web:".Length);
            // Split the remaining string by colon; per the DID method, each colon represents a path segment.
            var parts = didWithoutPrefix.Split(':');
            var domain = parts[0];
            string url;

            if (parts.Length == 1)
            {
                // No additional path segments; for did:web, the document lives at:
                // https://{domain}/.well-known/did.json
                url = $"https://{domain}/.well-known/did.json";
            }
            else
            {
                // Combine the remaining parts into a path.
                string path = string.Join("/", parts, 1, parts.Length - 1);
                // As per the did:web resolution rules, if the DID has additional segments,
                // the DID document is served from:
                // https://{domain}/{path}/.well-known/did.json
                url = $"https://{domain}/{path}/.well-known/did.json";
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var didDocument = JsonSerializer.Deserialize<DidDocument>(json);
            return didDocument;
        }
    }
}
