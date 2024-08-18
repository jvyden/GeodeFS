using System.Collections.Concurrent;
using System.Diagnostics;
using GeodeFS.Common.Networking;
using GeodeFS.Common.Networking.Packets;
using GeodeFS.Database;
using NotEnoughLogs;

namespace GeodeFS.Common.Federation;

public class FederationController : IDisposable
{
    private readonly Logger _logger;
    
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

    private readonly GeodeSqliteContext? _database;

    public FederationController(NetworkBackend networkBackend, Logger logger, GeodeSqliteContext? database = null)
    {
        this._networkBackend = networkBackend;
        networkBackend.OnPacket += HandlePacket;

        this._logger = logger;
        this._database = database;
    }

    private void HandlePacket(string source, IPacket packet)
    {
        this._logger.LogTrace(GeodeCategory.Peer, "Got packet {0} from peer {1}", packet.GetType().Name[6..], source);

        GeodeNode? node = this.Nodes.FirstOrDefault(n => n.Source == source);
        if (node == null)
        {
            if(packet is not PacketHandshake)
                throw new InvalidOperationException($"Cannot handle packets from an unknown node ({source} -> {this._networkBackend})");

            HandleHandshake(source);
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
            case PacketShareUser shareUser:
                if (shareUser.User.OriginatingNode == "local")
                    shareUser.User.OriginatingNode = source;
                
                this._database?.AddUser(shareUser.User.Pubkey, shareUser.User.PubkeyFingerprint, shareUser.User.OriginatingNode);
                this.ShareUser(source, shareUser.User);
                break;
            case PacketPing ping:
                this._networkBackend.SendPacket(source, new PacketPong(ping.TransactionId));
                break;
            default:
                throw new NotImplementedException(packet.GetType().ToString());
        }
    }

    private void HandleHandshake(string source)
    {
        GeodeNode node = new(source);
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
    }

    public void DiscoverNode(string source, GeodeNode newNode)
    {
        Debug.Assert(newNode is not GeodeLocalNode);
        this._logger.LogDebug(GeodeCategory.Peer, "Discovered node {0}", newNode.Source);
        
        foreach (GeodeNode node in this.DirectNodes)
        {
            if (node.Source == source) continue;
            this._networkBackend.SendPacket(node.Source, new PacketShareNode(newNode));
        }

        this.Nodes.Add(newNode);
    }

    public void ShareUser(string source, GeodeUser user)
    {
        foreach (GeodeNode node in this.DirectNodes)
        {
            if (node.Source == source) continue;
            this._networkBackend.SendPacket(node.Source, new PacketShareUser(user));
        }
    }

    public void HandshakeWithNode(string otherNode)
    {
        this._logger.LogInfo(GeodeCategory.Peer, "Handshaking with peer {0}", otherNode);
        this._currentlyHandshakingWith.Add(otherNode);
        this._networkBackend.Handshake(otherNode);
    }

    private GeodeNode GetDirectNode(string otherNode)
    {
        GeodeNode? node = this.DirectNodes.FirstOrDefault(n => n.Source == otherNode);
        if(node == null)
            throw new Exception("Not directly connected to node " + otherNode);

        return node;
    }

    public PacketPong? PingOtherNode(string otherNode)
    {
        GeodeNode node = GetDirectNode(otherNode);
        return PingOtherNode(node);
    }
    
    private PacketPong? PingOtherNode(GeodeNode otherNode)
    {
        PacketPong? result = this._networkBackend.DoTransaction<PacketPong>(otherNode, new PacketPing());
        otherNode.LastPing = DateTimeOffset.UtcNow;
        return result;
    }

    private Dictionary<GeodeNode, TResponse?> DoTransactionForAll<TResponse>(Func<ITransactionPacket> packetCreator)
        where TResponse : class, ITransactionPacket
    {
        ConcurrentDictionary<GeodeNode, TResponse?> results = new();
        
        Task[] tasks = new Task[this.DirectNodes.Count];
        for (int i = 0; i < this.DirectNodes.Count; i++)
        {
            GeodeNode node = this.DirectNodes[i];
            Task task = Task.Run(() =>
            {
                TResponse? result = this._networkBackend.DoTransaction<TResponse>(node, packetCreator());
                results.TryAdd(node, result);
            });
            tasks[i] = task;
        }

        Task.WaitAll(tasks);

        return results.ToDictionary();
    }
    
    private TResponse? DoTransactionForFirstResponse<TResponse>(Func<ITransactionPacket> packetCreator)
        where TResponse : class, ITransactionPacket
    {
        List<Task<TResponse?>> tasks = new(this.DirectNodes.Count);
        foreach (GeodeNode node in this.DirectNodes)
        {
            Task<TResponse?> task = Task.Run(() => this._networkBackend.DoTransaction<TResponse>(node, packetCreator()));
            tasks.Add(task);
        }
        
        TResponse? response = null;
        while (response == null && tasks.Count > 0)
        {
            Task<TResponse?> task = Task.WhenAny(tasks).Result;
            if (task.Result == null)
            {
                tasks.Remove(task);
                continue;
            }
            
            return task.Result;
        }

        return null;
    }

    public Dictionary<GeodeNode, PacketPong?> PingAllNodes() => DoTransactionForAll<PacketPong>(() => new PacketPing());

    public void Dispose()
    {
        if (this._networkBackend is IDisposable disposable)
            disposable.Dispose();
        
        GC.SuppressFinalize(this);
    }
}