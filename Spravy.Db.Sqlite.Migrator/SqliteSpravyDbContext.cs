using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spravy.Db.Contexts;
using Spravy.Db.Sqlite.Services;

namespace Spravy.Db.Sqlite.Migrator;

public class SqliteSpravyDbContext : SpravyDbContext
{
    public const string GenerateGuidQuery =
        "hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4) , 1)  || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))";

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