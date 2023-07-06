using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Db.Models;

namespace Spravy.Db.Contexts;

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