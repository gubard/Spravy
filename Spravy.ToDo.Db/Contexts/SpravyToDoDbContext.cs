using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.ToDo.Db.Contexts;

public class SpravyToDoDbContext : SpravyContext
{
    public const string GenerateGuidQuery =
        "hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4) , 1)  || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))";

    protected SpravyToDoDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyToDoDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }
}