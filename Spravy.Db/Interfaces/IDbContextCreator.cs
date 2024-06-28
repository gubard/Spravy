namespace Spravy.Db.Interfaces;

public interface IDbContextCreator<out TDbContext>
    where TDbContext : DbContext
{
    static abstract TDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options);
}
