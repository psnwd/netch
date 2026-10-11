using System.Security.Cryptography;

namespace Netch.Services;

/// <summary>
///     The release hash and the release file come from the same GitHub release, so a hash alone proves nothing.
///     Releases are also signed with a key that is not on GitHub; only the public half is in the app.
/// </summary>
public static class UpdateSignature
{
    /// <summary>
    ///     ECDSA P-256 public key (SubjectPublicKeyInfo, base64). The private half is the GitHub secret UPDATE_SIGNING_KEY, see sign.ps1.
    /// </summary>
    public const string PublicKey = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAExF1w9Fx4qd8sO2il0v6SxraifQGkZJfMjbAnB5Hherfi5lXcjRAj5N2WGIKR1+lYMy9IkmGynO/rwxiaga2Imw==";

    /// <param name="file">the downloaded archive</param>
    /// <param name="signature">contents of the release's "&lt;archive&gt;.sig" asset: base64 IEEE P1363 signature</param>
    public static bool Verify(string file, string signature, string publicKey = PublicKey)
    {
        try
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

            using var stream = File.OpenRead(file);
            return ecdsa.VerifyData(stream, Convert.FromBase64String(signature.Trim()), HashAlgorithmName.SHA256, DSASignatureFormat.IeeeP1363FixedFieldConcatenation);
        }
        catch (Exception e) when (e is FormatException or CryptographicException or ArgumentException)
        {
            return false;
        }
    }
}
