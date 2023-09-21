using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;

namespace Spravy.Db.Contexts;

public class SpravyContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyContext(IDbContextSetup setup)
    {
        this.setup = setup;
    }

    public SpravyContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        setup.OnModelCreating(modelBuilder);
    }
}