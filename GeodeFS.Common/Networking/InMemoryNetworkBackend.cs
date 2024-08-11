using GeodeFS.Common.Networking.Packets;

namespace GeodeFS.Common.Networking;

public class InMemoryNetworkBackend : NetworkBackend
{
    private readonly InMemoryNetwork _network;
    private readonly string _source;
    
    public InMemoryNetworkBackend(InMemoryNetwork network) : base()
    {
        this._network = network;
        this._network.Clients.Add(this);
        
        this._source = this._network.Clients.Count.ToString();
    }

    public override void SendPacket(string destination, IPacket packet)
    {
        InMemoryNetworkBackend? backend = this._network.Clients.FirstOrDefault(c => c._source == destination);

        Console.WriteLine($"{this._source}->{destination}: {packet.GetType().Name[6..]}");
        backend?.ReceivePacket(this, packet);
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