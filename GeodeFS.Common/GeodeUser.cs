namespace GeodeFS.Common;

#nullable disable

[MessagePackObject]
public class GeodeUser
{
    /// <summary>
    /// SHA-256
    /// </summary>
    [Key(0), MaxLength(64)]
    public string PubkeyHash { get; set; }
    [Key(1)]
    public string OriginatingNode { get; set; } // == GeodeNode.Source

    [Key(2)]
    public Dictionary<string, string> UserDetails { get; set; } = new(Maximum.DetailsPerUser);

    [Key(3)]
    public List<GeodeFileInfo> Files { get; set; } = new(Maximum.FilesPerUser);
}