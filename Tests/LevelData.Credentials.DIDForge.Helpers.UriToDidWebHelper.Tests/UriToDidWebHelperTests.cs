using LevelData.Credentials.DIDForge.Helpers;

namespace LevelData.Credentials.DIDForge.Services.Tests;

public class UriToDidWebHelperTests
{
    [Fact]
    public void ConvertUriToDidWeb_ValidHttpsUri_ShouldConvertCorrectly()
    {
        var uri = "https://example.com/keys/1";
        var expected = "did:web:example.com:keys:1";

        var result = UriToDidWebHelper.ConvertUriToDidWeb(uri);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("http://example.com")]
    [InlineData("ftp://example.com")]
    [InlineData("example.com")]
    [InlineData("")]
    [InlineData(null)]
    public void ConvertUriToDidWeb_InvalidUri_ShouldThrow(string uri)
    {
        var ex = Assert.Throws<ArgumentException>(() => UriToDidWebHelper.ConvertUriToDidWeb(uri));
        Assert.Contains("https://", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ConvertUriToDidWeb_UriWithTrailingSlash_ShouldConvertTrailingSlash()
    {
        var uri = "https://example.com/";
        var expected = "did:web:example.com:";

        var result = UriToDidWebHelper.ConvertUriToDidWeb(uri);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertUriToDidWeb_UriWithUpperCaseScheme_ShouldStillConvert()
    {
        var uri = "HTTPS://example.com/path";
        var expected = "did:web:example.com:path";

        var result = UriToDidWebHelper.ConvertUriToDidWeb(uri);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertUriToDidWeb_OnlyRootDomain_ShouldConvertCorrectly()
    {
        var uri = "https://example.com";
        var expected = "did:web:example.com";

        var result = UriToDidWebHelper.ConvertUriToDidWeb(uri);

        Assert.Equal(expected, result);
    }
}