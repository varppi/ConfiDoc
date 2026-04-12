using ConfidocLib;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace ConfidocTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PatchTest()
        {
            var patch = Difference.GetPatches("hello", "hello world34324");
            var patched = Difference.Patch("hello", patch);
            Assert.That((patched == "hello world34324"));
            
            patch = Difference.GetPatches("hello", "world34324");
            patched = Difference.Patch("hello", patch);
            Assert.That((patched == "world34324"));

            patch = Difference.GetPatches("hello", "world34324 hello");
            patched = Difference.Patch("hello", patch);
            Assert.That((patched == "world34324 hello"));
        }

        [Test]
        public void Signatures()
        {
            var keyPair = Security.GenerateKeyPair();
            var data = "hello world";
            var signature = Security.Sign(data, keyPair.PrivateKey!);
            Assert.That(Security.ValidSignature(data, signature, keyPair.PublicKey!));

            data = new string('f', 50000);
            signature = Security.Sign(data, keyPair.PrivateKey!);
            Assert.That(Security.ValidSignature(data, signature, keyPair.PublicKey!));
        }

        [Test]
        public void AES()
        {
            for (int i = 0; i< 1000; i++)
            {
                var keyPair = Security.GenerateKeyPair();
                var signature = Security.Sign("test data", keyPair.PrivateKey!);
                var enc = Security.EncryptKeyPair(keyPair, "hello world");
                var dec = Security.DecryptKeyPair(enc, "hello world");
                Assert.That(dec is not null);
                Assert.That(Security.ValidSignature("test data", signature, dec.PublicKey!));
                Assert.That(!Security.ValidSignature("test data", signature.Take(signature.Length-2).ToString()!, dec.PublicKey!));
                var encrypted1 = Security.Encrypt("test", "password");
                Assert.That(Security.Decrypt(encrypted1, "password") == "test");
            }
        }
    }
}
