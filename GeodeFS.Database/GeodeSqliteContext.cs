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
            DataSource = "geode.db",
            Mode = SqliteOpenMode.ReadWriteCreate
        }.ToString();
        
        optionsBuilder.UseSqlite(connectionString);
    }
}