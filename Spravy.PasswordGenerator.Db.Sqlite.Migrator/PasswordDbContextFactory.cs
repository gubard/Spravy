using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Interfaces;
using Spravy.PasswordGenerator.Db.Contexts;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator;

public class PasswordDbContextFactory : IFactory<string, PasswordDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public PasswordDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public PasswordDbContext Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(SpravyPasswordGeneratorDbSqliteMigratorMark.AssemblyFullName))
            .Options;

        return new PasswordDbContext(options, dbContextSetup);
    }
}