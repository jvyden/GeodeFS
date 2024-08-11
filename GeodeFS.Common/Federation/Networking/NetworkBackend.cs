namespace GeodeFS.Common.Federation.Networking;

public abstract class NetworkBackend
{
    public event Action<string, object>? OnPacket;

    public abstract void SendPacket(string destination, object packet);
    public abstract void Handshake(string source);

    protected void FireOnPacket(string source, object obj)
    {
        OnPacket?.Invoke(source, obj);
    }
}