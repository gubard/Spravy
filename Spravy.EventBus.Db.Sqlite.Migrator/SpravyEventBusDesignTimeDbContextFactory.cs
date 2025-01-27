using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spravy.EventBus.Db.Contexts;
using Spravy.EventBus.Db.Sqlite.Services;

namespace Spravy.EventBus.Db.Sqlite.Migrator;

public class SpravyEventBusDesignTimeDbContextFactory : IDesignTimeDbContextFactory<EventBusSpravyDbContext>
{
    public EventBusSpravyDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyEventBusDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new(options, new SqliteEventBusDbContextSetup());
    }
}