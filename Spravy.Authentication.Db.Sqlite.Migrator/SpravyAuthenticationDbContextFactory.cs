namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class SpravyAuthenticationDbContextFactory : IFactory<string, AuthenticationSpravyDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyAuthenticationDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<AuthenticationSpravyDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyAuthenticationDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new AuthenticationSpravyDbContext(options, dbContextSetup).ToResult();
    }
}