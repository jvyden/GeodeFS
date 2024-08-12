using PgpCore;

namespace GeodeFS.Common.Verification;

public static class PgpHelper
{
    public static bool VerifyUserMessage(GeodeUser user, string message)
    {
        EncryptionKeys keys = new(user.Pubkey);

        using PGP pgp = new(keys);
        return pgp.Verify(message);
    }
}