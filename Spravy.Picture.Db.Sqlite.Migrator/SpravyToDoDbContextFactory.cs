using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Picture.Db.Contexts;

namespace Spravy.Picture.Db.Sqlite.Migrator;

public class SpravyPictureDbContextFactory : IFactory<string, SpravyPictureDbContext>
{
    private readonly IDbContextSetup dbContextSetup;
    private readonly ILoggerFactory loggerFactory;

    public SpravyPictureDbContextFactory(IDbContextSetup dbContextSetup, ILoggerFactory loggerFactory)
    {
        this.dbContextSetup = dbContextSetup;
        this.loggerFactory = loggerFactory;
    }

    public Result<SpravyPictureDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
           .UseSqlite(key, b => b.MigrationsAssembly(SpravyPictureDbSqliteMigratorMark.AssemblyFullName))
           .UseLoggerFactory(loggerFactory)
           .Options;

        var context = new SpravyPictureDbContext(options, dbContextSetup);

        return context.ToResult();
    }
}