namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class
    SpravyAuthenticationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpravyDbAuthenticationDbContext>
{
    public SpravyDbAuthenticationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyAuthenticationDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new(options, new SqliteAuthenticationDbContextSetup());
    }
}