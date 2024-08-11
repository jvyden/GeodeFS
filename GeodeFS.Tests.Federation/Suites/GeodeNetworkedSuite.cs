using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking;

namespace GeodeFS.Tests.Federation.Suites;

public abstract class GeodeNetworkedSuite : GeodeTestSuite
{
    protected InMemoryNetwork Network = null!;
    
    [SetUp]
    public void SetUpNetwork()
    {
        this.Network = new InMemoryNetwork(false);   
    }

    protected FederationController CreateController(int id)
    {
        InMemoryNetworkBackend backend = new(this.Network, Proxy("net" + id));
        return new FederationController(backend, Proxy("con" + id));
    }
}