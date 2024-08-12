using System.ComponentModel.DataAnnotations;

namespace GeodeFS.Database.Models;

#nullable disable

public class DbGeodeUser
{
    /// <summary>
    /// SHA-256 hash of the <see cref="Pubkey"/>. Used for indexing/lookups
    /// </summary>
    [Key, MaxLength(256 / 8 * 2)]
    public string PubkeyHash { get; set; }
    
    [MaxLength(8192)]
    public string Pubkey { get; set; }
    [MaxLength(64)]
    public string OriginatingNode { get; set; }
}