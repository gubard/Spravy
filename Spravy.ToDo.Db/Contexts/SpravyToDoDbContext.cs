using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;

namespace Spravy.ToDo.Db.Contexts;

public class SpravyToDoDbContext : DbContext
{
    public const string GenerateGuidQuery =
        "hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4) , 1)  || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))";

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