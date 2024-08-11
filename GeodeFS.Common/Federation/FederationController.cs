using System.Diagnostics;
using GeodeFS.Common.Federation.Networking;

namespace GeodeFS.Common.Federation;

public class FederationController
{
    public readonly GeodeLocalNode LocalNode = new();
    public readonly List<GeodeNode> Nodes = [];

    private readonly NetworkBackend _networkBackend;

    public FederationController(NetworkBackend networkBackend)
    {
        _networkBackend = networkBackend;
    }

    public void DiscoverNode(GeodeNode newNode)
    {
        Debug.Assert(newNode is not GeodeLocalNode);
        
        foreach (GeodeNode node in Nodes)
        {
            _networkBackend.SendPacket(node.Source, newNode);
        }

        Nodes.Add(newNode);
    }

    public void HandshakeWithNode(string source)
    {
        _networkBackend.Handshake(source);
    }
}