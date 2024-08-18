using Bunkum.Core;
using Bunkum.Core.Database;
using GeodeFS.Database.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GeodeFS.Database;

public class GeodeSqliteContext : DbContext, IDatabaseContext
{
    private DbSet<DbGeodeUser> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(BunkumFileSystem.DataDirectory, "geode.db"),
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        
        optionsBuilder.UseSqlite(connectionString);
    }

    public void AddUser(string pubkey, string fingerprint, string node)
    {
        DbGeodeUser user = new()
        {
            Pubkey = pubkey,
            PubkeyFingerprint = fingerprint,
            OriginatingNode = node,
        };

        Users.Add(user);
        this.SaveChanges();
    }

    public DbGeodeUser? GetUserByFingerprint(string fingerprint)
    {
        return this.Users.FirstOrDefault(u => u.PubkeyFingerprint == fingerprint);
    }
}