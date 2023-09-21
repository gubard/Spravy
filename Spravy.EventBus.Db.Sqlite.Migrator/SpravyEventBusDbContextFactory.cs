using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.EventBus.Db.Contexts;

namespace Spravy.EventBus.Db.Sqlite.Migrator;

public class SpravyEventBusDbContextFactory : IFactory<string, SpravyEventBusDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyEventBusDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public SpravyEventBusDbContext Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(SpravyEventBusDbSqliteMigratorMark.AssemblyFullName))
            .Options;

        return new SpravyEventBusDbContext(options, dbContextSetup);
    }
}