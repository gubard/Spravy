using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Spravy.PasswordGenerator.Db.Contexts;
using Spravy.PasswordGenerator.Db.Sqlite.Services;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator;

public class SpravyUserSecretDesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserSecretDbContext>
{
    public UserSecretDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder().UseSqlite(
                args[0],
                b => b.MigrationsAssembly(SpravyPasswordGeneratorDbSqliteMigratorMark.AssemblyFullName)
            )
            .Options;

        return new UserSecretDbContext(options, new SqliteUserSecretDbContextSetup());
    }
}