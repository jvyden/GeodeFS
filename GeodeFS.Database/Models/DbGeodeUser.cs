using System.ComponentModel.DataAnnotations;

namespace GeodeFS.Database.Models;

#nullable disable

public class DbGeodeUser
{
    /// <summary>
    /// Fingerprint of the <see cref="Pubkey"/>. Used for indexing/lookups
    /// </summary>
    [Key, MaxLength(40)]
    public string PubkeyFingerprint { get; set; }
    
    [MaxLength(8192)]
    public string Pubkey { get; set; }
    [MaxLength(64)]
    public string OriginatingNode { get; set; }
}