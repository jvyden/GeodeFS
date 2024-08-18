using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking;

namespace GeodeFS.Tests.Suites;

public abstract class GeodeNetworkedSuite : GeodeTestSuite
{
    protected InMemoryNetwork Network = null!;
    protected virtual bool AutomaticNetwork => false;
    
    [SetUp]
    public void SetUpNetwork()
    {
        this.Network = new InMemoryNetwork(this.AutomaticNetwork);   
    }

    [TearDown]
    public void TearDownNetwork()
    {
        this.Network.Dispose();
    }

    protected FederationController CreateController(int id)
    {
        InMemoryNetworkBackend backend = new(this.Network, this.Proxy("net" + id));
        return new FederationController(backend, this.Proxy("con" + id));
    }
}