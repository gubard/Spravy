using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Schedule.Db.Contexts;

public class SpravyScheduleDbContext : SpravyContext, IDbContextCreator<SpravyScheduleDbContext>
{
    protected SpravyScheduleDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyScheduleDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static SpravyScheduleDbContext CreateContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new SpravyScheduleDbContext(options, setup);
    }
}