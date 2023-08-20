using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spravy.Authentication.Db.Contexts;
using Spravy.Authentication.Db.Sqlite.Services;

namespace Spravy.Authentication.Db.Sqlite.Migrator;

public class SqliteSpravyAuthenticationDbContext : SpravyAuthenticationDbContext
{
    public SqliteSpravyAuthenticationDbContext() : base(new SqliteAuthenticationDbContextSetup())
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite(configuration["Sqlite:ConnectionString"]);
    }
}