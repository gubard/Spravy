using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spravy.ToDo.Db.Sqlite.Services;
using Spravy.ToDo.Db.Contexts;

namespace Spravy.ToDo.Db.Sqlite.Migrator;

public class SqliteSpravyToDoDbContext : SpravyToDoDbContext
{
    public const string GenerateGuidQuery =
        "hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)), 2) || '-' || substr('AB89', 1 + (abs(random()) % 4) , 1)  || substr(hex(randomblob(2)), 2) || '-' || hex(randomblob(6))";

    public SqliteSpravyToDoDbContext() : base(new SqliteToDoDbContextSetup())
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite(configuration["Sqlite:ConnectionString"]);
    }
}