using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking;

namespace GeodeFS.Tests.Federation;

public class BasicFederationTests
{
    private InMemoryNetwork _network = null!;
    private FederationController _controller1 = null!;
    private FederationController _controller2 = null!;
    private FederationController _controller3 = null!;
    
    [SetUp]
    public void Setup()
    {
        _network = new InMemoryNetwork();
        
        _controller1 = new FederationController(new InMemoryNetworkBackend(_network));
        _controller2 = new FederationController(new InMemoryNetworkBackend(_network));
        _controller3 = new FederationController(new InMemoryNetworkBackend(_network));
    }

    [Test]
    public void DiscoversEachOther()
    {
        // 1 -> 2
        _controller1.HandshakeWithNode("2");

        Assert.Multiple(() =>
        {
            Assert.That(_controller1.Nodes.Count(n => n.Source == "2"), Is.EqualTo(1), "1 cant see 2");
            Assert.That(_controller1.DirectNodes.Count(n => n.Source == "2"), Is.EqualTo(1), "1 cant directly see 2");

            Assert.That(_controller2.Nodes.Count(n => n.Source == "1"), Is.EqualTo(1), "2 cant see 1");
            Assert.That(_controller2.DirectNodes.Count(n => n.Source == "1"), Is.EqualTo(1), "2 cant directly see 1");

            Assert.That(_controller3.Nodes, Is.Empty, "3 has nodes");
            Assert.That(_controller3.DirectNodes, Is.Empty, "3 has direct nodes");
            
            Assert.That(_controller1.Nodes.Count(n => n.Source == "3"), Is.Zero, "1 can see 3");
            Assert.That(_controller2.Nodes.Count(n => n.Source == "3"), Is.Zero, "2 can see 3");
        });
    }
    
    [Test]
    public void DiscoversEachOtherThroughForwardConnectingNodes()
    {
        // 1 -> 2 -> 3
        // ^---------^
        _controller1.HandshakeWithNode("2");
        _controller2.HandshakeWithNode("3");

        Assert.Multiple(() =>
        {
            Assert.That(_controller1.Nodes.Count(n => n.Source == "3"), Is.EqualTo(1), "1 cant see 3");
            Assert.That(_controller1.DirectNodes.Count(n => n.Source == "3"), Is.EqualTo(0), "1 can directly see 3");

            Assert.That(_controller3.Nodes.Count(n => n.Source == "1"), Is.EqualTo(1), "3 cant see 1");
            Assert.That(_controller3.DirectNodes.Count(n => n.Source == "1"), Is.EqualTo(0), "3 can directly see 1");
        });
    }
    
    [Test]
    public void DiscoversEachOtherThroughCentralNode()
    {
        // 1 -> 2 <- 3
        // ^---------^
        _controller1.HandshakeWithNode("2");
        _controller3.HandshakeWithNode("2");

        Assert.Multiple(() =>
        {
            Assert.That(_controller1.Nodes.Count(n => n.Source == "3"), Is.EqualTo(1), "1 cant see 3");
            Assert.That(_controller1.DirectNodes.Count(n => n.Source == "3"), Is.EqualTo(0), "1 can directly see 3");

            Assert.That(_controller3.Nodes.Count(n => n.Source == "1"), Is.EqualTo(1), "3 cant see 1");
            Assert.That(_controller3.DirectNodes.Count(n => n.Source == "1"), Is.EqualTo(0), "3 can directly see 1");
        });
    }
}