using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.ToDo.Db.Contexts;

public class SpravyDbToDoDbContext : SpravyDbContext, IDbContextCreator<SpravyDbToDoDbContext>
{
    public const string GenerateGuidQuery =
        "hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4) , 1)  || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))";

    protected SpravyDbToDoDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyDbToDoDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static SpravyDbToDoDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new SpravyDbToDoDbContext(options, setup);
    }
}