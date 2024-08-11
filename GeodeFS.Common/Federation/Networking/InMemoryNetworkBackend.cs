namespace GeodeFS.Common.Federation.Networking;

public class InMemoryNetworkBackend : NetworkBackend
{
    private readonly InMemoryNetwork _network;
    public string Source { get; set; }
    
    public InMemoryNetworkBackend(InMemoryNetwork network) : base()
    {
        this._network = network;
        this._network.Clients.Add(this);
        
        this.Source = this._network.Clients.Count.ToString();
    }

    public override void SendPacket(string destination, object packet)
    {
        InMemoryNetworkBackend? backend = this._network.Clients.FirstOrDefault(c => c.Source == destination);

        Console.WriteLine($"[{this.Source}]\tout to {destination}:\t{packet}");
        backend?.ReceivePacket(this, packet);
    }

    internal void ReceivePacket(InMemoryNetworkBackend source, object packet)
    {
        Console.WriteLine($"[{this.Source}]\tin from {source.Source}:\t{packet}");
        FireOnPacket(source.Source, packet);
    }

    public override void Handshake(string source)
    {
        SendPacket(source, "yo man");
    }
}