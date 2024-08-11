using GeodeFS.Common;
using GeodeFS.Server.Configuration;

namespace GeodeFS.Server.Endpoints;

public class UserEndpoints : EndpointGroup
{
    [ApiEndpoint("register", HttpMethods.Post)]
    [ApiEndpoint("register", HttpMethods.Get)]
    public GeodeUser CreateUser(RequestContext context, GeodeConfig config)
    {
        return new GeodeUser
        {
            OriginatingNode = config.NodeId.ToString(),
            PubkeyHash = "test"
        };
    }
}