using Microsoft.EntityFrameworkCore;
using Spravy.Db.Core.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Db.Contexts;

namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SpravyToDoDbContextFactory : IFactory<string, SpravyToDoDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyToDoDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public SpravyToDoDbContext Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName))
            .Options;

        return new SpravyToDoDbContext(options, dbContextSetup);
    }
}