using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.EventBus.Db.Contexts;

public class SpravyEventBusDbContext : SpravyContext, IDbContextCreator<SpravyEventBusDbContext>
{
    protected SpravyEventBusDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyEventBusDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static SpravyEventBusDbContext CreateContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new SpravyEventBusDbContext(options, setup);
    }
}