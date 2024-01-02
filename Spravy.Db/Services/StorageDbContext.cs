using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Db.Services;

public class StorageDbContext : SpravyDbContext
{
    public StorageDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public StorageDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}