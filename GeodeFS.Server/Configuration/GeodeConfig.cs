using Bunkum.Core.Configuration;

namespace GeodeFS.Server.Configuration;

public class GeodeConfig : Config
{
    public override int CurrentConfigVersion => 1;
    public override int Version { get; set; }
    protected override void Migrate(int oldVer, dynamic oldConfig)
    {}

    public Guid NodeId { get; set; } = Guid.NewGuid();
}