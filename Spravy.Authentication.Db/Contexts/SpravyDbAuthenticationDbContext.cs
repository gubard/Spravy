namespace Spravy.Authentication.Db.Contexts;

public class SpravyDbAuthenticationDbContext : SpravyDbContext
{
    protected SpravyDbAuthenticationDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyDbAuthenticationDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}