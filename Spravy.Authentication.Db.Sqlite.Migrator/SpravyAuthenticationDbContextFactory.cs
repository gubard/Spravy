using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Db.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class SpravyAuthenticationDbContextFactory : IFactory<string, SpravyDbAuthenticationDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyAuthenticationDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }
    
    public Result<SpravyDbAuthenticationDbContext> Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(SpravyAuthenticationDbSqliteMigratorMark.AssemblyFullName))
            .Options;

        return new SpravyDbAuthenticationDbContext(options, dbContextSetup).ToResult();
    }
}