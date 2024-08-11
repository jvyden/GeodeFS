using GeodeFS.Common;
using GeodeFS.Common.Federation;
using GeodeFS.Server.Configuration;

namespace GeodeFS.Server.Endpoints;

public class UserEndpoints : EndpointGroup
{
    [ApiEndpoint("register", HttpMethods.Post)]
    [ApiEndpoint("register", HttpMethods.Get)]
    public GeodeUser CreateUser(RequestContext context, GeodeLocalNode node)
    {
        return new GeodeUser
        {
            OriginatingNode = node.Source,
            PubkeyHash = "test"
        };
    }
}