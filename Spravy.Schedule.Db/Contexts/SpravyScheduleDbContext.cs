using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;

namespace Spravy.Schedule.Db.Contexts;

public class SpravyScheduleDbContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyScheduleDbContext(IDbContextSetup setup)
    {
        this.setup = setup;
    }

    public SpravyScheduleDbContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        setup.OnModelCreating(modelBuilder);
    }
}