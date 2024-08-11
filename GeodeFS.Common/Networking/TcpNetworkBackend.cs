using System.Net;
using System.Net.Sockets;
using GeodeFS.Common.Networking.Packets;
using NotEnoughLogs;

namespace GeodeFS.Common.Networking;

public class TcpNetworkBackend : NetworkBackend, IDisposable
{
    public const ushort DefaultPort = 36734;
    private readonly ushort _port;

    private readonly Socket _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly List<TcpPeer> _peers = [];

    public TcpNetworkBackend(Logger logger, ushort port = DefaultPort) : base(logger)
    {
        this._port = port;

        this._listener.Bind(new IPEndPoint(0, this._port));
        this._listener.Listen(100); // you just lost the game

        // ReSharper disable once UseObjectOrCollectionInitializer
        Task.Factory.StartNew(async () =>
        {
            while (!_disposed)
            {
                Socket client = await this._listener.AcceptAsync();
                this._peers.Add(new TcpPeer(this.FireOnPacket, client, true));
            }
        }, TaskCreationOptions.LongRunning);
    }

    public override void SendPacket(string destination, IPacket packet)
    {
        TcpPeer? peer = this._peers.FirstOrDefault(p => p.Source == destination);
        ObjectDisposedException.ThrowIf(peer == null, destination);
        
        peer.SendPacket(packet);
    }

    public override void Handshake(string source)
    {
        Socket server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Connect(IPEndPoint.Parse(source));

        TcpPeer peer = new(this.FireOnPacket, server, false);
        this._peers.Add(peer);
        
        peer.SendPacket(new PacketHandshake());
    }

    private bool _disposed;
    public void Dispose()
    {
        this._disposed = true;
        this._listener.Dispose();

        foreach (TcpPeer peer in this._peers)
            peer.Dispose();

        GC.SuppressFinalize(this);
    }
}