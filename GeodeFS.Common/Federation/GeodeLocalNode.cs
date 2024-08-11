namespace GeodeFS.Common.Federation;

public class GeodeLocalNode : GeodeNode
{
    public override string Source => "local";
    public override DateTimeOffset LastPing => DateTimeOffset.Now;
}