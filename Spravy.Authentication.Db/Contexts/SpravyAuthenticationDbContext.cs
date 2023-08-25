using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;

namespace Spravy.Authentication.Db.Contexts;

public class SpravyAuthenticationDbContext : DbContext
{
    private readonly IDbContextSetup setup;

    protected SpravyAuthenticationDbContext(IDbContextSetup setup)
    {
        this.setup = setup;
    }

    public SpravyAuthenticationDbContext(DbContextOptions options, IDbContextSetup setup) : base(options)
    {
        this.setup = setup;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        setup.OnModelCreating(modelBuilder);
    }
}