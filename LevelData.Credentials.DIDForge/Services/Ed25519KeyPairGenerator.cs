using LevelData.Credentials.DIDForge.Models;
using Multiformats.Base;
using Sodium;

namespace LevelData.Credentials.DIDForge.Services
{
    /// <summary>
    /// A helper class for generating Ed25519 key pairs.
    /// Replace the placeholder logic with a secure library call (e.g., using libsodium or BouncyCastle).
    /// </summary>
    public static class Ed25519KeyPairGenerator
    {
        public static Ed25519KeyPair GenerateKeyPair()
        {
            var keypair = PublicKeyAuth.GenerateKeyPair();
            keypair.PublicKey.ToString();

            // Placeholder for key generation logic.
            return new Ed25519KeyPair
            {
                PublicKeyBase58 = Base58EncodeEd25519PublicKey(keypair.PublicKey),
                PrivateKeyBase58 = Base58EncodeEd25519PrivateKey(keypair.PrivateKey)
            };
        }

        public static byte[] Base58DecodeString(string base58)
        {
            MultibaseEncoding encoding;
            byte[] result = Multibase.Decode(base58, out encoding, strict: true);
            if (encoding != MultibaseEncoding.Base58Btc)
            {
                throw new Exception("Unexpected encoding.");
            }

            return result;
        }

        public static string Base58EncodeBytes(byte[] publicKey)
        {
            return Multibase.Encode(MultibaseEncoding.Base58Btc, publicKey);
        }

        public static string Base58EncodeEd25519PublicKey(byte[] publicKey)
        {
            byte item = 237;
            List<byte> list = new List<byte> { item, 1 };
            list.AddRange(publicKey);
            return Base58EncodeBytes(list.ToArray());
        }

        public static string Base58EncodeEd25519PrivateKey(byte[] privateKey)
        {
            byte item = 19;
            List<byte> list = new List<byte> { item, 0 };
            list.AddRange(privateKey);
            return Base58EncodeBytes(list.ToArray());
        }

        public static string Sign(byte[] keyBlob, byte[] value)
        {
            try
            {
                byte[] signature = PublicKeyAuth.SignDetached(value, keyBlob);
                return Base58EncodeBytes(signature);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool Verify(byte[] publicKeyBlob, byte[] signature, byte[] originalData)
        {
            return PublicKeyAuth.VerifyDetached(signature, originalData, publicKeyBlob);
        }
    }
}
