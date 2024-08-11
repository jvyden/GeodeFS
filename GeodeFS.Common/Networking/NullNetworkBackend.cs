using GeodeFS.Common.Networking.Packets;
using NotEnoughLogs;

namespace GeodeFS.Common.Networking;

public class NullNetworkBackend : NetworkBackend
{
    public NullNetworkBackend(Logger logger) : base(logger)
    {}
    
    public override void SendPacket(string destination, IPacket packet)
    {
        
    }

    public override void Handshake(string source)
    {
        
    }
}