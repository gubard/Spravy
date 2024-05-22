namespace Spravy.Db.Services;

public class DbContextCacheValidator<TDbContext> : ICacheValidator<string, TDbContext>
    where TDbContext : SpravyDbContext
{
    public bool IsValid(string key, TDbContext value)
    {
        return !value.IsDisposed;
    }
}