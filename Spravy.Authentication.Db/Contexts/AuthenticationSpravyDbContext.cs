namespace Spravy.Authentication.Db.Contexts;

public class AuthenticationSpravyDbContext : SpravyDbContext
{
    protected AuthenticationSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public AuthenticationSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}