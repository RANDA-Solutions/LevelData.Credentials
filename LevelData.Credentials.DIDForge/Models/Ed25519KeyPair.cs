namespace LevelData.Credentials.DIDForge.Models
{
    /// <summary>
    /// Represents a simple Ed25519 key pair in Base58 format.
    /// Ensure that key material is securely generated and managed.
    /// </summary>
    public class Ed25519KeyPair
    {
        public string? Fragment { get; set; }
        public string PublicKeyBase58 { get; set; }
        public string PrivateKeyBase58 { get; set; }
    }

}
