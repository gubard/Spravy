namespace Spravy.PasswordGenerator.Db.Contexts;

public class UserSecretSpravyDbContext : SpravyDbContext
{
    protected UserSecretSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public UserSecretSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}