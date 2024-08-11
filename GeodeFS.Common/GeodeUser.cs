using System.ComponentModel.DataAnnotations;

namespace GeodeFS.Common;

#nullable disable

public class GeodeUser
{
    /// <summary>
    /// SHA-256
    /// </summary>
    [MaxLength(64)]
    public string PubkeyHash { get; set; }
    public string OriginatingNode { get; set; } // == GeodeNode.Source

    public Dictionary<string, string> UserDetails { get; set; } = new(Maximum.DetailsPerUser);

    public List<GeodeFileInfo> Files { get; set; } = new(Maximum.FilesPerUser);
}