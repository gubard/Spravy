using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.EventBus.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.EventBus.Db.Sqlite.Services;

public class SqliteEventBusDbContextSetup : IDbContextSetup
{
    public bool AutoCreateDataBase => false;

    public bool DataBaseCreated => true;

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}