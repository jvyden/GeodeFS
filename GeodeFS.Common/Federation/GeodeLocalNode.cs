namespace GeodeFS.Common.Federation;

public class GeodeLocalNode : GeodeNode
{
    public GeodeLocalNode() : base("local")
    {}

    public override DateTimeOffset LastPing => DateTimeOffset.Now;
}