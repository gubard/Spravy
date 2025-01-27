using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Db.Contexts;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator;

public class PasswordDbContextFactory : IFactory<string, PasswordSpravyDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public PasswordDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<PasswordSpravyDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyPasswordGeneratorDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new PasswordSpravyDbContext(options, dbContextSetup).ToResult();
    }
}