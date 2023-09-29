using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;

namespace Spravy.Db.Contexts;

public abstract class SpravyDbContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyDbContext(IDbContextSetup setup)
    {
        this.setup = setup;
    }

    protected SpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    public bool IsDisposed { get; private set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        setup.OnModelCreating(modelBuilder);
    }

    public override void Dispose()
    {
        IsDisposed = true;
        base.Dispose();
    }

    public override ValueTask DisposeAsync()
    {
        IsDisposed = true;

        return base.DisposeAsync();
    }
}