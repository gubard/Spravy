using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spravy.Picture.Db.Contexts;
using Spravy.Picture.Db.Sqlite.Services;

namespace Spravy.Picture.Db.Sqlite.Migrator;

public class SpravyPictureDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpravyPictureDbContext>
{
    public SpravyPictureDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyPictureDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new(options, new SqlitePictureDbContextSetup());
    }
}