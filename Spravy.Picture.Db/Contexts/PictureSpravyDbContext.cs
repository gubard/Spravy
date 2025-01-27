using Microsoft.EntityFrameworkCore;
using Spravy.Db.Contexts;
using Spravy.Db.Interfaces;

namespace Spravy.Picture.Db.Contexts;

public class PictureSpravyDbContext : SpravyDbContext, IDbContextCreator<PictureSpravyDbContext>
{
    protected PictureSpravyDbContext(IDbContextSetup setup) : base(setup)
    {
    }

    public PictureSpravyDbContext(DbContextOptions options, IDbContextSetup setup) : base(options, setup)
    {
    }

    public static PictureSpravyDbContext CreateDbContext(IDbContextSetup setup, DbContextOptions options)
    {
        return new(options, setup);
    }
}