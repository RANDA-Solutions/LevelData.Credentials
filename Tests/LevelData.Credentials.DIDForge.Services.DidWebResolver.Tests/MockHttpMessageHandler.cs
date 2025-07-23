namespace LevelData.Credentials.DIDForge.Services.Tests;

// Custom mock handler to intercept HTTP calls
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
    {
        _handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_handlerFunc(request));
    }
}