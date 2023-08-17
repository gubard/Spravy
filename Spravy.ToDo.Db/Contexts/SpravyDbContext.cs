using Microsoft.EntityFrameworkCore;
using Spravy.ToDo.Db.Interfaces;

namespace Spravy.ToDo.Db.Contexts;

public class SpravyDbContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyDbContext(IDbContextSetup setup)
    {
        this.setup = setup;
    }

    public SpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        setup.OnModelCreating(modelBuilder);
    }
}