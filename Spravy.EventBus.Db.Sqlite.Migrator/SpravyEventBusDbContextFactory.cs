using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.EventBus.Db.Contexts;

namespace Spravy.EventBus.Db.Sqlite.Migrator;

public class SpravyEventBusDbContextFactory : IFactory<string, EventBusSpravyDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyEventBusDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<EventBusSpravyDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyEventBusDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new EventBusSpravyDbContext(options, dbContextSetup).ToResult();
    }
}