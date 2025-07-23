using LevelData.Credentials.DIDForge.Abstractions;
using LevelData.Credentials.DIDForge.Models;
using Multiformats.Base;

namespace LevelData.Credentials.DIDForge.Services
{
    /// <summary>
    /// A simple resolver for did:key identifiers.
    /// 
    /// The did:key method encodes the public key material in the DID itself. For example:
    ///   did:key:z6Mkw... 
    /// This resolver assumes the key is an Ed25519 public key encoded in multibase (typically starting with "z").
    ///
    /// In a full implementation you would decode the multibase value, verify the multicodec prefix,
    /// extract the raw public key bytes, and then format the public key value appropriately.
    /// For simplicity, this example simply uses the extracted key portion directly.
    /// </summary>
    public class DidKeyResolver : IDidResolver
    {
        public Task<DidDocument> ResolveDidDocumentAsync(string did)
        {
            // Check that the DID starts with did:key:
            const string prefix = "did:key:";
            if (!did.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid DID format; expected did:key.", nameof(did));

            // Extract the key material (typically multibase encoded, e.g., starting with "z")
            // In this simple version, we assume the entire string after "did:key:" is the key.
            string keyMaterial = did.Substring(prefix.Length).Trim();
            if (Multibase.TryDecode(keyMaterial, out var encoding, out var rawKeyMaterial))
            {
                if (rawKeyMaterial == null || rawKeyMaterial.Length == 0)
                {
                    throw new ArgumentException("No key material found in DID; expected did:key:<key-material>", nameof(did));
                }
                // If the key material is already in Base58Btc encoding, we can use it directly.
                if (encoding != MultibaseEncoding.Base58Btc)
                {
                    // Decode the key material from multibase encoding
                    keyMaterial = Multibase.Encode(MultibaseEncoding.Base58Btc, rawKeyMaterial);
                }
            }
            // In a full implementation, you might decode keyMaterial (from its multibase encoding),
            // determine the key type via the multicodec prefix, and then extract the raw public key bytes.
            // Initially, we assume that:
            // - The key is an Ed25519 key.
            // - The did:key method encodes the public key in the DID.
            //
            // The verification method id will be the DID with a fragment appended.
            // According to common practice, the fragment can simply repeat the key material.
            string verificationMethodId = $"{did}#{keyMaterial}";

            var verificationMethod = new VerificationMethod
            {
                // The id of the verification method.
                Id = verificationMethodId,
                // Use the appropriate verification key type. This version uses the 2020 version.
                Type = "Ed25519VerificationKey2020",
                // The controller is the DID itself.
                Controller = did,
                // In a real resolver, this should be the properly formatted public key (e.g. Base58-encoded raw key material).
                PublicKeyMultibase = keyMaterial
            };

            // Build the DID document according to the DID Core specification.
            var didDocument = new DidDocument
            {
                Context = new List<string> { "https://www.w3.org/ns/did/v1",
                                             "https://w3id.org/security/suites/ed25519-2020/v1" },
                Id = did,
                VerificationMethod = new List<VerificationMethod> { verificationMethod },
                Authentication = new List<object> { verificationMethodId }
            };

            return Task.FromResult(didDocument);
        }
    }
}
