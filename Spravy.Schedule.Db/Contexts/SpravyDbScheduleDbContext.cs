using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Schedule.Db.Contexts;

public class SpravyDbScheduleDbContext
    : SpravyDbContext,
        IDbContextCreator<SpravyDbScheduleDbContext>
{
    protected SpravyDbScheduleDbContext(IDbContextSetup setup)
        : base(setup) { }

    public SpravyDbScheduleDbContext(DbContextOptions options, IDbContextSetup setup)
        : base(options, setup) { }

    public static SpravyDbScheduleDbContext CreateDbContext(
        IDbContextSetup setup,
        DbContextOptions options
    )
    {
        return new(options, setup);
    }
}
