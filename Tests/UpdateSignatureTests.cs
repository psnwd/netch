using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netrix.Services;

namespace Tests
{
    [TestClass]
    public class UpdateSignatureTests
    {
        private static (string Public, Func<string, string> Sign) NewKey()
        {
            var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            return (Convert.ToBase64String(key.ExportSubjectPublicKeyInfo()),
                file => Convert.ToBase64String(key.SignData(File.ReadAllBytes(file), HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation)));
        }

        private static string Archive(string content)
        {
            var file = Path.GetTempFileName();
            File.WriteAllText(file, content);
            return file;
        }

        [TestMethod]
        public void Valid_signature_passes()
        {
            var (pub, sign) = NewKey();
            var file = Archive("release");
            Assert.IsTrue(UpdateSignature.Verify(file, sign(file) + "\n", pub));
        }

        [TestMethod]
        public void Tampered_file_fails()
        {
            var (pub, sign) = NewKey();
            var file = Archive("release");
            var sig = sign(file);
            File.WriteAllText(file, "release with malware");
            Assert.IsFalse(UpdateSignature.Verify(file, sig, pub));
        }

        [TestMethod]
        public void Signature_from_another_key_fails()
        {
            var (pub, _) = NewKey();
            var (_, otherSign) = NewKey();
            var file = Archive("release");
            Assert.IsFalse(UpdateSignature.Verify(file, otherSign(file), pub));
        }

        [TestMethod]
        public void Garbage_signature_fails_instead_of_throwing()
        {
            var (pub, _) = NewKey();
            var file = Archive("release");
            Assert.IsFalse(UpdateSignature.Verify(file, "not base64!", pub));
            Assert.IsFalse(UpdateSignature.Verify(file, "", pub));
            Assert.IsFalse(UpdateSignature.Verify(file, Convert.ToBase64String(new byte[64]), pub));
        }
    }
}
