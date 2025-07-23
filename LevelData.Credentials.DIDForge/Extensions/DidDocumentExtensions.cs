using LevelData.Credentials.DIDForge.Models;
using System.Linq;
using System.Text.Json;

namespace LevelData.Credentials.DIDForge.Extensions
{
    public static class DidDocumentExtensions
    {
        /// <summary>
        /// Retrieves the PublicKeyMultibase from the DID Document for the specified key ID
        /// and verifies that the key is allowed for the specified use.
        /// </summary>
        /// <param name="didDocument">The DID Document.</param>
        /// <param name="keyId">The ID of the key to retrieve.</param>
        /// <param name="allowedUse">The allowed use to verify (e.g., "Authentication", "AssertionMethod").</param>
        /// <returns>The PublicKeyMultibase if the key is valid and allowed for the specified use.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the key is not found or not allowed for the specified use.</exception>
        public static string GetPublicKeyMultibaseForUse(this DidDocument didDocument, string keyId, string allowedUse)
        {
            if (didDocument == null)
            {
                throw new ArgumentNullException(nameof(didDocument));
            }

            // Find the verification method with the specified key ID
            var verificationMethod = didDocument.VerificationMethod?.FirstOrDefault(vm => vm.Id == keyId);
            if (verificationMethod == null)
            {
                throw new InvalidOperationException($"Key with ID '{keyId}' not found in the DID Document.");
            }

            // Check if the key is allowed for the specified use
            var isAllowed = allowedUse.ToLower() switch
            {
                "authentication" => IsKeyAllowed(didDocument.Authentication, keyId),
                "assertionmethod" => IsKeyAllowed(didDocument.AssertionMethod, keyId),
                "keyagreement" => IsKeyAllowed(didDocument.KeyAgreement, keyId),
                "capabilityinvocation" => IsKeyAllowed(didDocument.CapabilityInvocation, keyId),
                "capabilitydelegation" => IsKeyAllowed(didDocument.CapabilityDelegation, keyId),
                _ => throw new NotSupportedException($"The specified use '{allowedUse}' is not supported.")
            };

            if (!isAllowed)
            {
                throw new InvalidOperationException($"Key with ID '{keyId}' is not allowed for use '{allowedUse}'.");
            }

            // Return the PublicKeyMultibase
            return verificationMethod.PublicKeyMultibase;
        }

        /// <summary>
        /// Checks if the key is allowed for the specified use by inspecting the collection.
        /// </summary>
        /// <param name="collection">The collection to check (e.g., Authentication, AssertionMethod).</param>
        /// <param name="keyId">The key ID to check.</param>
        /// <returns>True if the key is allowed; otherwise, false.</returns>
        private static bool IsKeyAllowed(IEnumerable<object> collection, string keyId)
        {
            if (collection == null)
            {
                return false;
            }

            foreach (var item in collection)
            {
                if (item is string id && id == keyId)
                {
                    return true;
                }

                if (item is JsonElement element && element.ValueKind == JsonValueKind.String && element.GetString() == keyId)
                {
                    return true;
                }

                if (item is VerificationMethod method && method.Id == keyId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
