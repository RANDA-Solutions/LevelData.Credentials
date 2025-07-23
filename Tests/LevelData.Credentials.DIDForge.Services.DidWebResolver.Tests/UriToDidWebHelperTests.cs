using LevelData.Credentials.DIDForge.Helpers;
using LevelData.Credentials.DIDForge.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace LevelData.Credentials.DIDForge.Services.Tests;

public class DidWebResolverTests
{
    private HttpClient CreateMockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
    {
        var handler = new MockHttpMessageHandler(handlerFunc);
        return new HttpClient(handler);
    }

    [Fact]
    public async Task ResolveDidDocumentAsync_SinglePartDomain_ShouldRequestWellKnownPath()
    {
        var did = "did:web:example.com";
        var expectedUrl = "https://example.com/.well-known/did.json";

        var expectedDoc = new DidDocument { Id = did };
        var client = CreateMockHttpClient(request =>
        {
            Assert.Equal(expectedUrl, request.RequestUri.ToString());

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedDoc), Encoding.UTF8, "application/json")
            };
        });

        var resolver = new DidWebResolver(client);
        var result = await resolver.ResolveDidDocumentAsync(did);

        Assert.Equal(did, result.Id);
    }

    [Fact]
    public async Task ResolveDidDocumentAsync_MultiSegmentDid_ShouldRequestNestedWellKnownPath()
    {
        var did = "did:web:example.com:issuers:issuer123:request456";
        var expectedUrl = "https://example.com/issuers/issuer123/request456/.well-known/did.json";

        var expectedDoc = new DidDocument { Id = did };
        var client = CreateMockHttpClient(request =>
        {
            Assert.Equal(expectedUrl, request.RequestUri.ToString());

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(expectedDoc), Encoding.UTF8, "application/json")
            };
        });

        var resolver = new DidWebResolver(client);
        var result = await resolver.ResolveDidDocumentAsync(did);

        Assert.Equal(did, result.Id);
    }

    [Fact]
    public async Task ResolveDidDocumentAsync_InvalidDid_ShouldThrowArgumentException()
    {
        var client = CreateMockHttpClient(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var resolver = new DidWebResolver(client);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => resolver.ResolveDidDocumentAsync("did:key:xyz"));
        Assert.Contains("did:web", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ResolveDidDocumentAsync_HttpError_ShouldThrowHttpRequestException()
    {
        var did = "did:web:example.com";
        var client = CreateMockHttpClient(_ =>
            new HttpResponseMessage(HttpStatusCode.NotFound)
        );

        var resolver = new DidWebResolver(client);

        await Assert.ThrowsAsync<HttpRequestException>(() => resolver.ResolveDidDocumentAsync(did));
    }
}
