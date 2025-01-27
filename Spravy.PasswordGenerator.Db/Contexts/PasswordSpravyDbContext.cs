namespace Spravy.PasswordGenerator.Db.Contexts;

public class PasswordSpravyDbContext : SpravyDbContext, IDbContextCreator<PasswordSpravyDbContext>
{
    protected PasswordSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public PasswordSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static PasswordSpravyDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}