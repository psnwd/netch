# Signs an update archive for the in-app updater (Netch/Services/UpdateSignature.cs verifies it).
#   .\sign.ps1 -GenerateKey              make a key pair: prints the public key, writes the private key to update-signing.key
#   .\sign.ps1 -File Netch.7z            writes Netch.7z.sig; private key from $Env:UPDATE_SIGNING_KEY (base64 PKCS#8)
# ECDSA P-256 + SHA-256, IEEE P1363 signature, base64. Keep the private key in the GitHub secret UPDATE_SIGNING_KEY only.
param (
    [string]$File,
    [switch]$GenerateKey
)

$ErrorActionPreference = 'Stop'
$ecdsa = [Security.Cryptography.ECDsa]::Create()

if ( $GenerateKey ) {
    $ecdsa.GenerateKey([Security.Cryptography.ECCurve]::CreateFromFriendlyName('nistP256'))
    [IO.File]::WriteAllText("$PWD\update-signing.key", [Convert]::ToBase64String($ecdsa.ExportPkcs8PrivateKey()))
    Write-Host 'Public key (put it in UpdateSignature.PublicKey):'
    Write-Host ([Convert]::ToBase64String($ecdsa.ExportSubjectPublicKeyInfo()))
    Write-Host 'Private key written to update-signing.key: add it as the secret UPDATE_SIGNING_KEY, then delete the file.'
    exit 0
}

if ( -Not $File -or -Not $Env:UPDATE_SIGNING_KEY ) { throw 'Usage: UPDATE_SIGNING_KEY=<base64> .\sign.ps1 -File <archive>' }

[void]$ecdsa.ImportPkcs8PrivateKey([Convert]::FromBase64String($Env:UPDATE_SIGNING_KEY), [ref]$null)
$sig = $ecdsa.SignData([IO.File]::ReadAllBytes((Resolve-Path $File)), [Security.Cryptography.HashAlgorithmName]::SHA256, [Security.Cryptography.DSASignatureFormat]::IeeeP1363FixedFieldConcatenation)
[IO.File]::WriteAllText("$File.sig", [Convert]::ToBase64String($sig))
Write-Host "Signed $File"
