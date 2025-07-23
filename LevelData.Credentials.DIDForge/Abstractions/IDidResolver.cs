using LevelData.Credentials.DIDForge.Models;

namespace LevelData.Credentials.DIDForge.Abstractions
{
    /// <summary>
    /// Interface for DID document resolvers.
    /// </summary>
    public interface IDidResolver
    {
        /// <summary>
        /// Resolves a DID document given a did:web identifier.
        /// </summary>
        /// <param name="did">The did:web identifier.</param>
        /// <returns>The resolved DID document.</returns>
        Task<DidDocument> ResolveDidDocumentAsync(string did);
    }
}
