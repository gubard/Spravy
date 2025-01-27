using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Schedule.Db.Contexts;

namespace Spravy.Schedule.Db.Sqlite.Migrator;

public class SpravyScheduleDbContextFactory : IFactory<string, ScheduleSpravyDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyScheduleDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<ScheduleSpravyDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyScheduleDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new ScheduleSpravyDbContext(options, dbContextSetup).ToResult();
    }
}