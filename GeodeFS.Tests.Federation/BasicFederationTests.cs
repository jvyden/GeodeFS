using GeodeFS.Common.Federation;
using GeodeFS.Common.Federation.Networking;

namespace GeodeFS.Tests.Federation;

public class BasicFederationTests
{
    private InMemoryNetwork _network = null!;
    private FederationController _controller1 = null!;
    private FederationController _controller2 = null!;
    
    [SetUp]
    public void Setup()
    {
        _network = new InMemoryNetwork();
        
        _controller1 = new FederationController(new InMemoryNetworkBackend(_network));
        _controller2 = new FederationController(new InMemoryNetworkBackend(_network));
    }

    [Test]
    public void DiscoversEachOther()
    {
        _controller1.HandshakeWithNode("2");
    }
}