using System.Diagnostics;
using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking.Packets;
using GeodeFS.Tests.Suites;

namespace GeodeFS.Tests.Federation.Tests;

public class TransactionTests : GeodeNetworkedSuite
{
    private FederationController _controller1 = null!;
    private FederationController _controller2 = null!;

    protected override bool AutomaticNetwork => true;

    [SetUp]
    public void Setup()
    {
        this._controller1 = CreateController(1);
        this._controller2 = CreateController(2);
        
        this._controller1.HandshakeWithNode("2");
        
        this.Network.ProcessTasks();
    }

    [Test]
    public void DoesPingTransaction()
    {
        PacketPong? result = this._controller2.PingOtherNode("1");
        Debug.Assert(result != null);
    }

    [Test]
    public void PingsAllNodes()
    {
        // set up third node
        FederationController controller3 = CreateController(3);
        controller3.HandshakeWithNode("1");
        this.Network.ProcessTasks();

        Dictionary<GeodeNode, PacketPong?> results = this._controller1.PingAllNodes();
        Assert.That(results, Has.Count.EqualTo(2));
        Assert.That(results.All(r => r.Value != null), Is.True);
    }
}