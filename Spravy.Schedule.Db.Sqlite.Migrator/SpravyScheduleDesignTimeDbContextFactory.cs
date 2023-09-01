using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spravy.Schedule.Db.Contexts;
using Spravy.Schedule.Db.Sqlite.Services;

namespace Spravy.Schedule.Db.Sqlite.Migrator;

public class SpravyScheduleDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpravyScheduleDbContext>
{
    public SpravyScheduleDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyScheduleDbSqliteMigratorMark.AssemblyFullName)
            )
            .Options;

        return new SpravyScheduleDbContext(options, new SqliteScheduleDbContextSetup());
    }
}