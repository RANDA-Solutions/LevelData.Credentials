namespace LevelData.Credentials.DIDForge.Services.Tests;

public class DidKeyResolverTests
{
    private readonly DidKeyResolver _resolver = new DidKeyResolver();

    [Fact]
    public async Task ResolveDidDocumentAsync_ValidDidKey_ShouldReturnValidDocument()
    {
        // Example multibase-encoded Ed25519 public key
        var keyMaterial = "z6MkszFTBPLf7WmxQEN2QBDzEvYZykDiA1oUJ5AHDw2vQYKn";
        var did = $"did:key:{keyMaterial}";

        var doc = await _resolver.ResolveDidDocumentAsync(did);

        Assert.NotNull(doc);
        Assert.Equal(did, doc.Id);
        Assert.Contains("https://www.w3.org/ns/did/v1", doc.Context);
        Assert.Single(doc.VerificationMethod);

        var method = doc.VerificationMethod[0];
        Assert.Equal($"{did}#{keyMaterial}", method.Id);
        Assert.Equal("Ed25519VerificationKey2020", method.Type);
        Assert.Equal(did, method.Controller);
        Assert.Equal(keyMaterial, method.PublicKeyMultibase);
        Assert.Contains(method.Id, doc.Authentication);
    }

    [Theory]
    [InlineData("did:web:example.com")]
    [InlineData("did:key")] // missing colon
    [InlineData("did:key:")] // no key material
    [InlineData("did:key:   ")] // whitespace key
    public async Task ResolveDidDocumentAsync_InvalidDidKey_ShouldThrow(string did)
    {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _resolver.ResolveDidDocumentAsync(did));

        Assert.Contains("did:key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ResolveDidDocumentAsync_ValidButShortKey_ShouldStillProduceValidDidDocument()
    {
        var keyMaterial = "z123";
        var did = $"did:key:{keyMaterial}";

        var doc = await _resolver.ResolveDidDocumentAsync(did);

        Assert.Equal(did, doc.Id);
        Assert.Equal($"{did}#{keyMaterial}", doc.VerificationMethod[0].Id);
        Assert.Equal(keyMaterial, doc.VerificationMethod[0].PublicKeyMultibase);
    }

    [Fact]
    public async Task ResolveDidDocumentAsync_MultibaseDecodeShouldNotThrow()
    {
        var keyMaterial = "z6MkszFTBPLf7WmxQEN2QBDzEvYZykDiA1oUJ5AHDw2vQYKn"; // valid base58btc multibase
        var did = $"did:key:{keyMaterial}";

        var exception = await Record.ExceptionAsync(() =>
            _resolver.ResolveDidDocumentAsync(did));

        Assert.Null(exception);
    }
}
