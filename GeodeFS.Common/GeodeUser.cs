using GeodeFS.Database.Models;

namespace GeodeFS.Common;

#nullable disable

[MessagePackObject]
public class GeodeUser
{
    public GeodeUser()
    {}

    public GeodeUser(DbGeodeUser user)
    {
        this.PubkeyFingerprint = user.PubkeyFingerprint;
        this.Pubkey = user.Pubkey;
        this.OriginatingNode = user.OriginatingNode;
    }

    /// <summary>
    /// Fingerprint of the <see cref="Pubkey"/>. Used for indexing/lookups
    /// </summary>
    [Key(0), MaxLength(40)]
    public string PubkeyFingerprint { get; set; }
    
    [Key(1)]
    public string Pubkey { get; set; }

    [Key(2)]
    public string OriginatingNode { get; set; } // == GeodeNode.Source

    [Key(3)]
    public Dictionary<string, string> UserDetails { get; set; } = new(Maximum.DetailsPerUser);

    [Key(4)]
    public List<GeodeFileInfo> Files { get; set; } = new(Maximum.FilesPerUser);
}