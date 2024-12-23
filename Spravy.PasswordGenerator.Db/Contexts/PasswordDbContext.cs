namespace Spravy.PasswordGenerator.Db.Contexts;

public class PasswordDbContext : SpravyDbContext, IDbContextCreator<PasswordDbContext>
{
    protected PasswordDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public PasswordDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static PasswordDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}