using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Sqlite.Services;

namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class SpravyAuthenticationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpravyDbAuthenticationDbContext>
{
    public SpravyDbAuthenticationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyAuthenticationDbSqliteMigratorMark.AssemblyFullName)
            )
            .Options;

        return new SpravyDbAuthenticationDbContext(options, new SqliteAuthenticationDbContextSetup());
    }
}