using LevelData.Credentials.DIDForge.Abstractions;
using LevelData.Credentials.DIDForge.Helpers;
using LevelData.Credentials.DIDForge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelData.Credentials.DIDForge.Services
{
    /// <summary>
    /// Generates a did:web DID Document that contains an Ed25519 verification method.
    /// The DID will follow a pattern like: 
    /// did:web:domain:issuers:issuerId:requestId 
    /// and the corresponding verification method fragment will be appended (e.g., "#key-1").
    /// </summary>
    public class DidDocumentGenerator : IDidDocumentGenerator
    {
        /// <summary>
        /// Generates a DID document by generating a new key internally.
        /// </summary>
        public DidDocument GenerateDidDocument(string uri)
        {
            // Generate a new signing key (Ed25519) for this request.
            // Replace this placeholder with an actual cryptographic key pair generator,
            // for example using libsodium or BouncyCastle.
            var keyPair = Ed25519KeyPairGenerator.GenerateKeyPair();

            // Construct the did:web string.  
            // According to the DID spec, the method-specific-id is a colon-separated list; 
            // it will be resolved to a URL by transforming colons into path delimiters.
            string did = UriToDidWebHelper.ConvertUriToDidWeb(uri);
            
            var verificationMethod = AddVerificationMethod(did, keyPair);

            // Create and return the DID document.
            var didDocument = new DidDocument
            {
                Context = new List<string> { "https://www.w3.org/ns/did/v1",
                                             "https://w3id.org/security/suites/ed25519-2020/v1" },
                Id = did,
                VerificationMethod = new List<VerificationMethod> { verificationMethod },
                Authentication = new List<object> { verificationMethod.Id }
            };

            return didDocument;
        }

        /// <summary>
        /// Generates a DID document using externally supplied keys.
        /// For each key provided, a corresponding verification method is created with a unique fragment.
        /// </summary>
        public DidDocument GenerateDidDocument(string uri, List<Ed25519KeyPair> providedKeys)
        {
            if (providedKeys == null || !providedKeys.Any())
                throw new ArgumentException("A non-empty list of keys must be provided.", nameof(providedKeys));

            // Construct the did:web string.
            string did = UriToDidWebHelper.ConvertUriToDidWeb(uri);

            var didDocument = new DidDocument
            {
                Context = new List<string> { "https://www.w3.org/ns/did/v1",
                                             "https://w3id.org/security/suites/ed25519-2020/v1" },
                Id = did
            };

            var verificationMethods = new List<VerificationMethod>();
            foreach (var keyPair in providedKeys)
            {
                didDocument.AddVerificationMethod(AddVerificationMethod(did, keyPair));
            }
            return didDocument;
        }

        public VerificationMethod AddVerificationMethod(string did, Ed25519KeyPair keyPair)
        {
            // Generate a unique verification method ID
            string verificationMethodId = $"{did}#{keyPair.Fragment ?? $"key-{Guid.NewGuid(),8:N}"}";
            var verificationMethod = new VerificationMethod
            {
                Id = verificationMethodId,
                Type = "Ed25519VerificationKey2020",
                Controller = did,
                PublicKeyMultibase = keyPair.PublicKeyBase58
            };
            return verificationMethod;
        }


    }
}
