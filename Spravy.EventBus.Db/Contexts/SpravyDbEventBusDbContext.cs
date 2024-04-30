using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.EventBus.Db.Contexts;

public class SpravyDbEventBusDbContext : SpravyDbContext, IDbContextCreator<SpravyDbEventBusDbContext>
{
    protected SpravyDbEventBusDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyDbEventBusDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static SpravyDbEventBusDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}