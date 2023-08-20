using Microsoft.EntityFrameworkCore;
using Spravy.Db.Core.Interfaces;

namespace Spravy.ToDo.Db.Contexts;

public class SpravyToDoDbContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyToDoDbContext(IDbContextSetup setup)
    {
        this.setup = setup;
    }

    public SpravyToDoDbContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        setup.OnModelCreating(modelBuilder);
    }
}