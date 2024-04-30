using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.PasswordGenerator.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.PasswordGenerator.Db.Sqlite.Services;

public class SqliteUserSecretDbContextSetup : IDbContextSetup
{
    public bool AutoCreateDataBase
    {
        get => false;
    }

    public bool DataBaseCreated
    {
        get => true;
    }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserSecretEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}