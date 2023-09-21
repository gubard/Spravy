using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Schedule.Db.Contexts;

public class SpravyScheduleDbContext : SpravyContext
{
    protected SpravyScheduleDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyScheduleDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}