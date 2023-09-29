using Spravy.Db.Contexts;
using Spravy.Domain.Interfaces;

namespace Spravy.Db.Services;

public class DbContextCacheValidator<TDbContext> : ICacheValidator<string, TDbContext>
    where TDbContext : SpravyDbContext
{
    public bool IsValid(string key, TDbContext value)
    {
        return !value.IsDisposed;
    }
}