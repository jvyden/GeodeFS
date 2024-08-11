namespace GeodeFS.Common.Federation;

#nullable disable

public class GeodeNode
{
    public GeodeNode(string source)
    {
        Source = source;
    }

    public virtual string Source { get; }
    public virtual DateTimeOffset LastPing { get; set; } = DateTimeOffset.Now;
}