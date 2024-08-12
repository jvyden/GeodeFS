using Bunkum.Core.Endpoints.Debugging;
using GeodeFS.Api;
using GeodeFS.Common;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Verification;
using GeodeFS.Database;

namespace GeodeFS.Server.Endpoints;

public class UserEndpoints : EndpointGroup
{
    [ApiEndpoint("register", HttpMethods.Post)]
    [NullStatusCode(Forbidden)]
    public GeodeUser? CreateUser(RequestContext context, GeodeLocalNode node, GeodeSqliteContext database, RegistrationRequest body)
    {
        if (!PgpHelper.VerifyUserMessage(body.Pubkey, body.Message, "register"))
            return null;

        string fingerprint = PgpHelper.GetFingerprint(body.Pubkey);

        GeodeUser user = new()
        {
            OriginatingNode = node.Source,
            Pubkey = body.Pubkey,
            PubkeyFingerprint = fingerprint,
        };

        database.AddUser(user.Pubkey, user.PubkeyFingerprint, node.Source);

        return user;
    }
}