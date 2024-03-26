using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.PasswordGenerator.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.PasswordGenerator.Db.Sqlite.Services;

public class SqlitePasswordDbContextSetup : IDbContextSetup
{
    public bool AutoCreateDataBase => false;
    public bool DataBaseCreated => true;
    
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PasswordItemEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}