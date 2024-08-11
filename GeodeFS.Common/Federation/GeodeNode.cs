namespace GeodeFS.Common.Federation;

#nullable disable

public class GeodeNode
{
    public virtual string Source { get; set; }
    public virtual DateTimeOffset LastPing { get; set; }
}