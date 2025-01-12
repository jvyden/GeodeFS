using GeodeFS.Common.Networking.Packets;
using NotEnoughLogs;

namespace GeodeFS.Common.Networking;

public class InMemoryNetworkBackend : NetworkBackend
{
    private readonly InMemoryNetwork _network;
    private readonly string _source;
    
    public InMemoryNetworkBackend(InMemoryNetwork network, Logger logger) : base(logger)
    {
        this._network = network;
        this._network.Clients.Add(this);
        
        this._source = this._network.Clients.Count.ToString();
    }

    public override void SendPacket(string destination, IPacket packet)
    {
        InMemoryNetworkBackend? backend = this._network.Clients.FirstOrDefault(c => c._source == destination);

        _network.QueueTask(() =>
        {
            Logger.LogTrace(GeodeCategory.Peer, $"Sending packet {packet.GetType().Name[6..]} to {destination}");
            backend?.ReceivePacket(this, packet);
        });
    }

    private void ReceivePacket(InMemoryNetworkBackend source, IPacket packet)
    {
        // Console.WriteLine($"[{this._source}]\tin from {source._source}:\t{packet.GetType().Name[6..]}");
        FireOnPacket(source._source, packet);
    }

    public override void Handshake(string source)
    {
        SendPacket(source, new PacketHandshake());
    }

    public override string ToString() => _source;
}