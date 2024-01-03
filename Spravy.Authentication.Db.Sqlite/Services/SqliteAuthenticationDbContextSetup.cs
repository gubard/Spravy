using Microsoft.EntityFrameworkCore;
using Spravy.Authentication.Db.Sqlite.EntityTypeConfigurations;
using Spravy.Db.Interfaces;

namespace Spravy.Authentication.Db.Sqlite.Services;

public class SqliteAuthenticationDbContextSetup : IDbContextSetup
{
    public bool AutoCreateDataBase => false;
    public bool DataBaseCreated => true;

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}