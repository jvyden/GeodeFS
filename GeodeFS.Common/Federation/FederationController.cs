using System.Diagnostics;
using GeodeFS.Common.Networking;
using GeodeFS.Common.Networking.Packets;

namespace GeodeFS.Common.Federation;

public class FederationController
{
    /// <summary>
    /// This node is us.
    /// </summary>
    public readonly GeodeLocalNode LocalNode = new();
    /// <summary>
    /// The full network we are aware of.
    /// </summary>
    public readonly List<GeodeNode> Nodes = [];
    /// <summary>
    /// Nodes we are directly connected to.
    /// </summary>
    public readonly List<GeodeNode> DirectNodes = [];

    private readonly NetworkBackend _networkBackend;

    private readonly List<string> _currentlyHandshakingWith = [];

    public FederationController(NetworkBackend networkBackend)
    {
        _networkBackend = networkBackend;
        networkBackend.OnPacket += HandlePacket;
    }

    private void HandlePacket(string source, IPacket packet)
    {
        GeodeNode? node = this.Nodes.FirstOrDefault(n => n.Source == source);
        if (node == null)
        {
            if(packet is not PacketHandshake)
                throw new InvalidOperationException($"Cannot handle packets from an unknown node ({source} -> {this._networkBackend})");

            node = new GeodeNode(source);
            this.DirectNodes.Add(node);

            if (!this._currentlyHandshakingWith.Remove(source))
            {
                // this client is connecting to us. send back a handshake to finish things up
                this._networkBackend.SendPacket(node, new PacketHandshake());
            }
            
            this.DiscoverNode(source, node);
            
            // tell the other side the current state
            foreach (GeodeNode otherNode in this.Nodes)
            {
                if(node.Source == otherNode.Source)
                    continue;

                this._networkBackend.SendPacket(node, new PacketShareNode(otherNode));
            }
            
            return;
        }

        switch (packet)
        {
            case PacketShareNode shareNode:
                this.DiscoverNode(source, new GeodeNode(shareNode.Source)
                {
                    Relations = shareNode.Relations,
                });
                break;
            default:
                throw new NotImplementedException(packet.GetType().ToString());
        }
    }

    public void DiscoverNode(string source, GeodeNode newNode)
    {
        Debug.Assert(newNode is not GeodeLocalNode);
        Console.WriteLine("Discovered node " + newNode.Source);
        
        foreach (GeodeNode node in this.DirectNodes)
        {
            if (node.Source == source) continue;
            this._networkBackend.SendPacket(node.Source, new PacketShareNode(newNode));
        }

        this.Nodes.Add(newNode);
    }

    public void HandshakeWithNode(string source)
    {
        Console.WriteLine($"Handshaking with {source}");
        this._currentlyHandshakingWith.Add(source);
        this._networkBackend.Handshake(source);
    }
}