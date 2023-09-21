using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.EventBus.Db.Contexts;

public class SpravyEventBusDbContext : SpravyContext
{
    protected SpravyEventBusDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyEventBusDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}