namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class
    SpravyAuthenticationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthenticationSpravyDbContext>
{
    public AuthenticationSpravyDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyAuthenticationDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new(options, new SqliteAuthenticationDbContextSetup());
    }
}