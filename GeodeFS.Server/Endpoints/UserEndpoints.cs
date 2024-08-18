using Bunkum.Core.Endpoints.Debugging;
using GeodeFS.Api;
using GeodeFS.Common;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Verification;
using GeodeFS.Database;
using GeodeFS.Database.Models;

namespace GeodeFS.Server.Endpoints;

public class UserEndpoints : EndpointGroup
{
    [ApiEndpoint("register", HttpMethods.Post)]
    [NullStatusCode(Forbidden)]
    public GeodeUser? CreateUser(RequestContext context, FederationController con, GeodeSqliteContext database, RegistrationRequest body)
    {
        if (!PgpHelper.VerifyUserMessage(body.Pubkey, body.Message, "register"))
            return null;

        string fingerprint = PgpHelper.GetFingerprint(body.Pubkey);

        GeodeUser user = new()
        {
            OriginatingNode = con.LocalNode.Source,
            Pubkey = body.Pubkey,
            PubkeyFingerprint = fingerprint,
        };

        database.AddUser(user.Pubkey, user.PubkeyFingerprint, con.LocalNode.Source);
        con.ShareUser(con.LocalNode.Source, user);

        return user;
    }

    [ApiEndpoint("user/{fingerprint}")]
    public GeodeUser? GetUser(RequestContext context, GeodeSqliteContext database, string fingerprint)
    {
        DbGeodeUser? user = database.GetUserByFingerprint(fingerprint);
        if (user == null) return null;

        return new GeodeUser(user);
    }
}