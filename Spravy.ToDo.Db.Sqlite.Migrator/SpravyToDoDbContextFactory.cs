using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Db.Contexts;

namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SpravyToDoDbContextFactory : IFactory<string, SpravyDbToDoDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyToDoDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<SpravyDbToDoDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName)
            )
            .Options;

        return new SpravyDbToDoDbContext(options, dbContextSetup).ToResult();
    }
}
