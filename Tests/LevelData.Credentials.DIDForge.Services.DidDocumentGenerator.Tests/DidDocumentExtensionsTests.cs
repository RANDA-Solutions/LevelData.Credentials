using System;
using System.Collections.Generic;
using System.Text.Json;
using LevelData.Credentials.DIDForge.Extensions;
using LevelData.Credentials.DIDForge.Models;
using Xunit;

namespace LevelData.Credentials.DIDForge.Extensions.Tests
{
    public class DidDocumentExtensionsTests
    {
        private const string SampleDidDocumentJson = @"{""@context"": [""https://www.w3.org/ns/did/v1"", ""https://w3id.org/security/suites/ed25519-2020/v1""], ""id"": ""did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355"", ""verificationMethod"": [{""id"": ""did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064"", ""type"": ""Ed25519VerificationKey2020"", ""controller"": ""did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355"", ""publicKeyMultibase"": ""z6MknMNXzmZwPcskMVww7Ts5nh7geT55mJ41nG98roMkN7ku""}], ""authentication"": [""did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064""], ""assertionMethod"": [""did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064""]}";

        private static DidDocument ParseSampleDidDocument()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<DidDocument>(SampleDidDocumentJson, options);
        }

        [Fact]
        public void GetPublicKeyMultibaseForUse_ValidAuthenticationKey_ReturnsPublicKeyMultibase()
        {
            var doc = ParseSampleDidDocument();
            var keyId = "did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064";
            var result = doc.GetPublicKeyMultibaseForUse(keyId, "authentication");
            Assert.Equal("z6MknMNXzmZwPcskMVww7Ts5nh7geT55mJ41nG98roMkN7ku", result);
        }

        [Fact]
        public void GetPublicKeyMultibaseForUse_ValidAssertionMethodKey_ReturnsPublicKeyMultibase()
        {
            var doc = ParseSampleDidDocument();
            var keyId = "did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064";
            var result = doc.GetPublicKeyMultibaseForUse(keyId, "assertionMethod");
            Assert.Equal("z6MknMNXzmZwPcskMVww7Ts5nh7geT55mJ41nG98roMkN7ku", result);
        }

        [Fact]
        public void GetPublicKeyMultibaseForUse_KeyNotInVerificationMethod_Throws()
        {
            var doc = ParseSampleDidDocument();
            var keyId = "did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-NOTFOUND";
            Assert.Throws<InvalidOperationException>(() => doc.GetPublicKeyMultibaseForUse(keyId, "authentication"));
        }

        [Fact]
        public void GetPublicKeyMultibaseForUse_KeyNotAllowedForUse_Throws()
        {
            var doc = ParseSampleDidDocument();
            var keyId = "did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064";
            Assert.Throws<InvalidOperationException>(() => doc.GetPublicKeyMultibaseForUse(keyId, "capabilityInvocation"));
        }

        [Fact]
        public void GetPublicKeyMultibaseForUse_AllowedUseCaseInsensitive_Works()
        {
            var doc = ParseSampleDidDocument();
            var keyId = "did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064";
            var result = doc.GetPublicKeyMultibaseForUse(keyId, "AsSeRtIoNmEtHoD");
            Assert.Equal("z6MknMNXzmZwPcskMVww7Ts5nh7geT55mJ41nG98roMkN7ku", result);
        }

        [Fact]
        public void GetPublicKeyMultibaseForUse_UnsupportedUse_Throws()
        {
            var doc = ParseSampleDidDocument();
            var keyId = "did:web:randaocpservice-test.azurewebsites.net:issuers:360beaba-2a51-4733-a410-60cb303a6355#key-23c3cf59b73a4c12adab99d2f7424064";
            Assert.Throws<NotSupportedException>(() => doc.GetPublicKeyMultibaseForUse(keyId, "notARealUse"));
        }
    }
}
