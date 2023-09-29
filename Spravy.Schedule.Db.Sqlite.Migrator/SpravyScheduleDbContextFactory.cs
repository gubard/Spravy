using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.Schedule.Db.Contexts;

namespace Spravy.Schedule.Db.Sqlite.Migrator;

public class SpravyScheduleDbContextFactory : IFactory<string, SpravyDbScheduleDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyScheduleDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public SpravyDbScheduleDbContext Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(SpravyScheduleDbSqliteMigratorMark.AssemblyFullName))
            .Options;

        return new SpravyDbScheduleDbContext(options, dbContextSetup);
    }
}