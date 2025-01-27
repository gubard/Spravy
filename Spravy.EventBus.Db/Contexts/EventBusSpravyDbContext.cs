using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.EventBus.Db.Contexts;

public class EventBusSpravyDbContext : SpravyDbContext, IDbContextCreator<EventBusSpravyDbContext>
{
    protected EventBusSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public EventBusSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static EventBusSpravyDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}