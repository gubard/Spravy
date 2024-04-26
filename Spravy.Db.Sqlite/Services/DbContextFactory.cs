using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Db.Sqlite.Services;

public class DbContextFactory<TDbContext, TAssemblyMark> : IFactory<string, TDbContext>
    where TDbContext : DbContext, IDbContextCreator<TDbContext>
    where TAssemblyMark : IAssemblyMark
{
    private readonly IDbContextSetup dbContextSetup;

    public DbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<TDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(TAssemblyMark.AssemblyFullName))
            .Options;

        var context = TDbContext.CreateDbContext(dbContextSetup, options);

        return context.ToResult();
    }
}