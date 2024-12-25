using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Picture.Db.Contexts;

public class SpravyPictureDbContext : SpravyDbContext, IDbContextCreator<SpravyPictureDbContext>
{
    protected SpravyPictureDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public SpravyPictureDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static SpravyPictureDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}