using System.Text;

namespace LevelData.Credentials.DIDForge.Services.Tests;

public class Ed25519KeyPairGeneratorTests
{
    [Fact]
    public void GenerateKeyPair_ShouldReturnValidBase58Keys()
    {
        var keyPair = Ed25519KeyPairGenerator.GenerateKeyPair();

        Assert.False(string.IsNullOrWhiteSpace(keyPair.PublicKeyBase58), "Public key should not be null or empty.");
        Assert.False(string.IsNullOrWhiteSpace(keyPair.PrivateKeyBase58), "Private key should not be null or empty.");
    }

    [Fact]
    public void Base58EncodeDecode_ShouldRoundTripCorrectly()
    {
        var originalBytes = Encoding.UTF8.GetBytes("Hello World!");
        var encoded = Ed25519KeyPairGenerator.Base58EncodeBytes(originalBytes);
        var decoded = Ed25519KeyPairGenerator.Base58DecodeString(encoded);

        Assert.Equal(originalBytes, decoded);
    }

    [Fact]
    public void PublicKeyEncoding_ShouldStartWithExpectedPrefix()
    {
        var keyPair = Ed25519KeyPairGenerator.GenerateKeyPair();
        var decoded = Ed25519KeyPairGenerator.Base58DecodeString(keyPair.PublicKeyBase58);

        Assert.Equal((byte)237, decoded[0]);
        Assert.Equal(1, decoded[1]);
        Assert.Equal(34, decoded.Length); // 2 prefix bytes + 32 byte public key
    }

    [Fact]
    public void PrivateKeyEncoding_ShouldStartWithExpectedPrefix()
    {
        var keyPair = Ed25519KeyPairGenerator.GenerateKeyPair();
        var decoded = Ed25519KeyPairGenerator.Base58DecodeString(keyPair.PrivateKeyBase58);

        Assert.Equal((byte)19, decoded[0]);
        Assert.Equal(0, decoded[1]);
        Assert.Equal(66, decoded.Length); // 2 prefix bytes + 64 byte private key
    }

    [Fact]
    public void SignAndVerify_ShouldReturnTrueForValidSignature()
    {
        var keyPair = Ed25519KeyPairGenerator.GenerateKeyPair();

        var privateKeyBlob = Ed25519KeyPairGenerator.Base58DecodeString(keyPair.PrivateKeyBase58)[2..]; // strip prefix
        var publicKeyBlob = Ed25519KeyPairGenerator.Base58DecodeString(keyPair.PublicKeyBase58)[2..];

        var message = Encoding.UTF8.GetBytes("This is a test message.");

        var signatureBase58 = Ed25519KeyPairGenerator.Sign(privateKeyBlob, message);
        var signature = Ed25519KeyPairGenerator.Base58DecodeString(signatureBase58);

        var isValid = Ed25519KeyPairGenerator.Verify(publicKeyBlob, signature, message);

        Assert.True(isValid, "Signature should be valid for the given message and public key.");
    }

    [Fact]
    public void Verify_ShouldReturnFalseForTamperedMessage()
    {
        var keyPair = Ed25519KeyPairGenerator.GenerateKeyPair();

        var privateKeyBlob = Ed25519KeyPairGenerator.Base58DecodeString(keyPair.PrivateKeyBase58)[2..];
        var publicKeyBlob = Ed25519KeyPairGenerator.Base58DecodeString(keyPair.PublicKeyBase58)[2..];

        var message = Encoding.UTF8.GetBytes("Original message.");
        var tamperedMessage = Encoding.UTF8.GetBytes("Tampered message.");

        var signatureBase58 = Ed25519KeyPairGenerator.Sign(privateKeyBlob, message);
        var signature = Ed25519KeyPairGenerator.Base58DecodeString(signatureBase58);

        var isValid = Ed25519KeyPairGenerator.Verify(publicKeyBlob, signature, tamperedMessage);

        Assert.False(isValid, "Signature should not be valid for a tampered message.");
    }
}
