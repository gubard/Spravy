using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Contexts;
using Spravy.Db.Interfaces;
using Spravy.Domain.Interfaces;

namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class SpravyAuthenticationDbContextFactory : IFactory<string, SpravyAuthenticationDbContext>
{
    private readonly IDbContextSetup dbContextSetup;

    public SpravyAuthenticationDbContextFactory(IDbContextSetup dbContextSetup)
    {
        this.dbContextSetup = dbContextSetup;
    }
    
    public SpravyAuthenticationDbContext Create(string key)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite(key, b => b.MigrationsAssembly(SpravyAuthenticationDbSqliteMigratorMark.AssemblyFullName))
            .Options;

        return new SpravyAuthenticationDbContext(options, dbContextSetup);
    }
}