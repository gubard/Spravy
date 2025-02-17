using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Db.Contexts;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator;

public class UserSecretDbContextFactory : IFactory<string, UserSecretSpravyDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public UserSecretDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }

    public Result<UserSecretSpravyDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                key,
                b => b.MigrationsAssembly(SpravyPasswordGeneratorDbSqliteMigratorMark.AssemblyFullName)
            )
           .Options;

        return new UserSecretSpravyDbContext(options, dbContextSetup).ToResult();
    }
}