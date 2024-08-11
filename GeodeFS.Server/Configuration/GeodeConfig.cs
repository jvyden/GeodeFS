using Bunkum.Core.Configuration;
using GeodeFS.Common.Networking;

namespace GeodeFS.Server.Configuration;

public class GeodeConfig : Config
{
    public override int CurrentConfigVersion => 3;
    public override int Version { get; set; }
    protected override void Migrate(int oldVer, dynamic oldConfig)
    {}

    public ushort ListenPort { get; set; } = TcpNetworkBackend.DefaultPort;
    public List<string> DirectPeers { get; set; } = [];
}