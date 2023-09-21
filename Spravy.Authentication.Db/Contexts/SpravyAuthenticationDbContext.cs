using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Authentication.Db.Contexts;

public class SpravyAuthenticationDbContext : SpravyContext
{
    protected SpravyAuthenticationDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyAuthenticationDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}