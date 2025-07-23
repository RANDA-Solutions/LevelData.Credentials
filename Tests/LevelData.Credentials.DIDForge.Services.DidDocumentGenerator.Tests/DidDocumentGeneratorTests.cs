using LevelData.Credentials.DIDForge.Models;

namespace LevelData.Credentials.DIDForge.Services.Tests;

public class DidDocumentGeneratorTests
{
    private readonly DidDocumentGenerator _generator = new DidDocumentGenerator();

    [Fact]
    public void GenerateDidDocument_InternalKey_ShouldGenerateValidDocument()
    {
        var doc = _generator.GenerateDidDocument("https://example.com/issuers/issuer123/request456");

        Assert.NotNull(doc);
        Assert.Equal("did:web:example.com:issuers:issuer123:request456", doc.Id);
        Assert.Contains("https://www.w3.org/ns/did/v1", doc.Context);
        Assert.Single(doc.VerificationMethod);

        var method = doc.VerificationMethod.First();
        Assert.StartsWith(doc.Id + "#", method.Id);
        Assert.Equal(doc.Id, method.Controller);
        Assert.Equal("Ed25519VerificationKey2020", method.Type);
        Assert.False(string.IsNullOrWhiteSpace(method.PublicKeyMultibase));

        Assert.Contains(method.Id, doc.Authentication);
    }

    [Fact]
    public void GenerateDidDocument_ProvidedKeys_ShouldGenerateMultipleVerificationMethods()
    {
        var key1 = Ed25519KeyPairGenerator.GenerateKeyPair();
        key1.Fragment = "key1";

        var key2 = Ed25519KeyPairGenerator.GenerateKeyPair();
        key2.Fragment = "key2";

        var keys = new List<Ed25519KeyPair> { key1, key2 };

        var doc = _generator.GenerateDidDocument("https://example.com/issuers/issuerXYZ/req789", keys);

        Assert.NotNull(doc);
        Assert.Equal(2, doc.VerificationMethod.Count);
        Assert.All(doc.VerificationMethod, vm => Assert.StartsWith(doc.Id + "#", vm.Id));
        Assert.Contains(doc.VerificationMethod, vm => vm.Id.EndsWith("#key1"));
        Assert.Contains(doc.VerificationMethod, vm => vm.Id.EndsWith("#key2"));
    }

    [Fact]
    public void GenerateDidDocument_WithEmptyProvidedKeys_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _generator.GenerateDidDocument("https://example.com/issuer/request", new List<Ed25519KeyPair>())
        );

        Assert.Contains("A non-empty list of keys must be provided", ex.Message);
    }

    [Fact]
    public void AddVerificationMethod_ShouldGenerateExpectedFormat_WhenNoFragment()
    {
        var key = Ed25519KeyPairGenerator.GenerateKeyPair();
        key.Fragment = null; // force use of default Guid

        var did = "did:web:example.com:issuer:test";
        var method = _generator.AddVerificationMethod(did, key);

        Assert.StartsWith(did + "#key-", method.Id);
        Assert.Equal("Ed25519VerificationKey2020", method.Type);
        Assert.Equal(did, method.Controller);
        Assert.Equal(key.PublicKeyBase58, method.PublicKeyMultibase);
    }

    [Fact]
    public void AddVerificationMethod_ShouldUseProvidedFragment()
    {
        var key = Ed25519KeyPairGenerator.GenerateKeyPair();
        key.Fragment = "my-fragment";

        var did = "did:web:example.com:issuer:test";
        var method = _generator.AddVerificationMethod(did, key);

        Assert.Equal($"{did}#my-fragment", method.Id);
    }
}
