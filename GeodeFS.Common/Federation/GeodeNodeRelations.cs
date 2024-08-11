namespace GeodeFS.Common.Federation;

[MessagePackObject]
public class GeodeNodeRelations
{
    [Key(0)]
    public List<string> KnowsOf { get; set; } = [];
    [Key(1)]
    public List<string> HeardFrom { get; set; } = [];
}