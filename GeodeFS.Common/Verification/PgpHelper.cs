using Org.BouncyCastle.Bcpg.OpenPgp;
using PgpCore;
using PgpCore.Models;

namespace GeodeFS.Common.Verification;

public static class PgpHelper
{
    public static string GetFingerprint(string pubkey)
    {
        EncryptionKeys keys = new(pubkey);
        PgpPublicKey? key = keys.PublicKeyRings.FirstOrDefault()?.PgpPublicKeyRing.GetPublicKeys().FirstOrDefault();
        if (key == null)
            throw new InvalidOperationException("Keyring contains no keys.");

        return CryptoHelper.BytesToHexString(key.GetFingerprint());
    }
    
    public static bool VerifyUserMessage(string pubkey, string message)
    {
        EncryptionKeys keys = new(pubkey);

        using PGP pgp = new(keys);
        return pgp.VerifyClearArmoredString(message);
    }
    
    public static bool VerifyUserMessage(string pubkey, string message, string expectedText)
    {
        EncryptionKeys keys = new(pubkey);

        using PGP pgp = new(keys);
        VerificationResult result = pgp.VerifyAndReadClearArmoredString(message);

        return result.IsVerified && result.ClearText.TrimEnd(['\r', '\n']) == expectedText;
    }
}