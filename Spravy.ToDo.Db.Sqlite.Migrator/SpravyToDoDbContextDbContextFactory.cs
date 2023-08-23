using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Spravy.ToDo.Db.Contexts;
using Spravy.ToDo.Db.Sqlite.Services;

namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SpravyToDoDbContextDbContextFactory : IDesignTimeDbContextFactory<SpravyToDoDbContext>
{
    public SpravyToDoDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var options = new DbContextOptionsBuilder().UseSqlite(
                configuration.GetSection("Sqlite:ConnectionString").Value,
                b => b.MigrationsAssembly(SpravyToDoDbSqliteMigratorMark.AssemblyFullName)
            )
            .Options;

        return new SpravyToDoDbContext(options, new SqliteToDoDbContextSetup());
    }
}