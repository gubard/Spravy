using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.PasswordGenerator.Db.Contexts;

public class UserSecretDbContext : SpravyDbContext
{
    protected UserSecretDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public UserSecretDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}