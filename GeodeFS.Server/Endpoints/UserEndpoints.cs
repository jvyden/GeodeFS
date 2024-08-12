using Bunkum.Core.Endpoints.Debugging;
using GeodeFS.Api;
using GeodeFS.Common;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Verification;

namespace GeodeFS.Server.Endpoints;

public class UserEndpoints : EndpointGroup
{
    [ApiEndpoint("register", HttpMethods.Post)]
    [NullStatusCode(Forbidden)]
    public GeodeUser? CreateUser(RequestContext context, GeodeLocalNode node, RegistrationRequest body)
    {
        if (!PgpHelper.VerifyUserMessage(body.Pubkey, body.Message, "register"))
            return null;

        string fingerprint = PgpHelper.GetFingerprint(body.Pubkey);
        
        return new GeodeUser
        {
            OriginatingNode = node.Source,
            Pubkey = body.Pubkey,
            PubkeyFingerprint = fingerprint,
        };
    }
}