using GeodeFS.Common.Networking.Packets;

namespace GeodeFS.Common.Networking;

public class NullNetworkBackend : NetworkBackend
{
    public override void SendPacket(string destination, IPacket packet)
    {
        
    }

    public override void Handshake(string source)
    {
        
    }
}