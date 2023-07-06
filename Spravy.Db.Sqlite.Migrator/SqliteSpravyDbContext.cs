using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spravy.Db.Contexts;
using Spravy.Db.Sqlite.Services;

namespace Spravy.Db.Sqlite.Migrator;

public class SqliteSpravyDbContext : SpravyDbContext
{
    public SqliteSpravyDbContext() : base(new SqliteDbContextSetup())
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite(configuration["Sqlite:ConnectionString"]);
    }
}