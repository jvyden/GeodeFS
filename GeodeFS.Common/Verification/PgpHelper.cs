using PgpCore;
using PgpCore.Models;

namespace GeodeFS.Common.Verification;

public static class PgpHelper
{
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