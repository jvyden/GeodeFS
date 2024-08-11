using GeodeFS.Common.Federation;
using GeodeFS.Common.Networking.Packets;
using NotEnoughLogs;

namespace GeodeFS.Common.Networking;

public abstract class NetworkBackend
{
    protected readonly Logger Logger;

    protected NetworkBackend(Logger logger)
    {
        this.Logger = logger;
    }

    public event Action<string, IPacket>? OnPacket;

    public abstract void SendPacket(string destination, IPacket packet);

    public void SendPacket(GeodeNode node, IPacket packet) => this.SendPacket(node.Source, packet);
    public abstract void Handshake(string source);

    protected void FireOnPacket(string source, IPacket obj)
    {
        OnPacket?.Invoke(source, obj);
    }
}