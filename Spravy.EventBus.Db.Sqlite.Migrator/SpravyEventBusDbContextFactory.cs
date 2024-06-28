using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.EventBus.Db.Contexts;

namespace Spravy.EventBus.Db.Sqlite.Migrator;

public class SpravyEventBusDbContextFactory : IFactory<string, SpravyDbEventBusDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyEventBusDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<SpravyDbEventBusDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyEventBusDbSqliteMigratorMark.AssemblyFullName)
            )
            .Options;

        return new SpravyDbEventBusDbContext(options, dbContextSetup).ToResult();
    }
}
