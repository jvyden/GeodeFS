using Bunkum.EntityFrameworkDatabase;

namespace GeodeFS.Database;

public class GeodeDbProvider : EntityFrameworkDatabaseProvider<GeodeSqliteContext>
{
    protected override EntityFrameworkInitializationStyle InitializationStyle => EntityFrameworkInitializationStyle.EnsureCreated;
}