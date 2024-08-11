using GeodeFS.Common.Federation;

namespace GeodeFS.Common.Networking.Packets;

#nullable disable

[MessagePackObject]
public class PacketShareNode : IPacket
{
    public PacketShareNode()
    {}

    public PacketShareNode(GeodeNode node)
    {
        this.Source = node.Source;
        this.Relations = node.Relations;
    }

    [Key(0)]
    public string Source { get; set; }
    
    [Key(1)]
    public GeodeNodeRelations Relations { get; set; }
}