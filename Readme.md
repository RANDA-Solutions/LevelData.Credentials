# LevelData.Credentials.DIDForge

A .NET library for generating and resolving [Decentralized Identifier (DID)](https://www.w3.org/TR/did-1.0/) documents for both **did:key** and **did:web** methods. The library produces spec‑compliant DID Documents, decodes/verifies multibase & multicodec prefixes, and supports serialization with both `Newtonsoft.Json` and `System.Text.Json`.

---

## Features

- **did:key Resolver**: Parse a `did:key:z…` identifier, decode Base58 multibase, validate the Ed25519 multicodec prefix, and build a DID Document.
- **did:web Resolver**: Map a `did:web` identifier to its well-known URL and fetch the DID Document over HTTP.
- **DID Document Generation**:
  - Internal key generation via `Ed25519KeyPairGenerator`.
  - External key provisioning via `DidDocumentGenerator.GenerateDidDocument(...)` overload.
- **Spec-Compliant Model**: All standard DID Document properties (`id`, `controller`, `verificationMethod`, `authentication`, `assertionMethod`, `keyAgreement`, `capabilityInvocation`, `capabilityDelegation`, `service`).
- **Dual Serializer Support**: Attributes for both `Newtonsoft.Json` (`[JsonProperty]`) and `System.Text.Json` (`[JsonPropertyName]`, `[JsonIgnore(Condition=WhenWritingNull)]`).

---

## Installation

1. **Add as a project reference**
   ```xml
   <ProjectReference Include="../path/to/LevelData.Did/LevelData.Did.csproj" />
   ```

2. **(Optional) Package via NuGet**
   Publish the library as a NuGet package named `LevelData.Did` and install:
   ```bash
   dotnet add package LevelData.Did
   ```

3. **Dependencies**
   - .NET Standard Library 2.0
   - `Newtonsoft.Json` (>=13.x)
   - `System.Text.Json` (>=9.x)
   - `Multiformats.Base` (2.0.2)
   - `Sodium.Core` (1.4.0)
   - `Microsoft.Extensions.DependencyInjection` (>=3.1.32)
---

## Core Types

```csharp
namespace LevelData.Credentials.DIDForge
{
  // DID document model
  public class DidDocument { /* id, controller, etc. */ }
  public class VerificationMethod { /* id, type, controller, publicKeyBase58 */ }
  public class Service { /* id, type, serviceEndpoint */ }

  // Resolvers
  public interface IDidResolver { Task<DidDocument> ResolveDidDocumentAsync(string did); }
  public class DidKeyResolver : IDidResolver { /* resolves did:key */ }
  public class DidWebResolver : IDidResolver { /* resolves did:web via HttpClient */ }

  // Generator for did:web
  public interface IDidDocumentGenerator { /* two overloads */ }
  public class DidDocumentGenerator : IDidDocumentGenerator { /* internal/external keys */ }

  // Helper
  public static class UriToDidWebHelper { /* ConvertUriToDidWeb methods */ }
}
```

---

## Quickstart

### Resolving a did:key

```csharp
using LevelData.Credentials.DIDForge;

var resolver = new DidKeyResolver();
var document = await resolver.ResolveDidDocumentAsync(
    "did:key:z6MkkAJUqP1x7PmRNCkH1w6i1mFVhz1QTxtyvchVji3BpLHK"
);

Console.WriteLine(
    JsonConvert.SerializeObject(document, Formatting.Indented)
);

// Or with System.Text.Json:
string json = JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true });
```

### Resolving a did:web

```csharp
using LevelData.Credentials.DIDForge;
using System.Net.Http;

var httpClient = new HttpClient();
var webResolver = new DidWebResolver(httpClient);
var webDid = "did:web:example.com:issuers:1234:abcd";
var webDoc = await webResolver.ResolveDidDocumentAsync(webDid);

Console.WriteLine(webDoc.Context[0]); // https://www.w3.org/ns/did/v1
```

### Generating a did:web DID Document

```csharp
using LevelData.Credentials.DIDForge;

IDidDocumentGenerator generator = new DidDocumentGenerator();

// 1) Internal key generation:
var doc1 = generator.GenerateDidDocument("https://example.com/issuers/1234/abcd");

// 2) External keys provided by caller:
var externalKeys = new List<Ed25519KeyPair> { /* your key pairs */ };
var doc2 = generator.GenerateDidDocument(
    "https://example.com/issuers/1234/abcd",
    externalKeys
);
```

### Getting a key from a DID Document
```csharp
var didDocument = await didResolver.GetResolver("did:web:example.com").ResolveDidDocumentAsync("did:web:example.com");

// Specify the key ID and the allowed use
string keyId = "did:web:example.com#key-1";
string allowedUse = "Authentication";

try
{
    string publicKeyMultibase = didDocument.GetPublicKeyMultibaseForUse(keyId, allowedUse);
    Console.WriteLine($"PublicKeyMultibase: {publicKeyMultibase}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Add support for DID Resolver to Dependency Injection
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDidResolvers();
    // Other service registrations...
}

```
---

## Extending & Customization

- **Additional Key Types**: Extend `DidKeyResolver` to support other multicodec prefixes (e.g., X25519, Secp256k1).
- **Additional Relationships**: Populate `AssertionMethod`, `KeyAgreement`, etc., in `DidDocument` as needed.
- **Service Endpoints**: Add entries to the `Service` list to advertise REST APIs, messaging endpoints, etc.
- **Caching & Validation**: Wrap `DidWebResolver` in a caching decorator or integrate signature validation on fetched documents.

---

## Contribution

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature/YourFeature`).
3. Commit your changes and push.
4. Open a Pull Request describing your work.

---



© LevelData

