using LevelData.Credentials.DIDForge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelData.Credentials.DIDForge.Abstractions
{
    /// <summary>
    /// Interface for a component that generates DID documents.
    /// </summary>
    public interface IDidDocumentGenerator
    {
        /// <summary>
        /// Generates a DID document.
        /// </summary>
        /// <param name="uri">The uri for the issuer (e.g., "https://randaocpservice-test.azurewebsites.net/issuer/issuer-id")</param>
        /// <returns>A DID document instance.</returns>
        DidDocument GenerateDidDocument(string uri);
        DidDocument GenerateDidDocument(string uri, List<Ed25519KeyPair> providedKeys);
    }
}
