using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.EventBus.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.EventBus.Db.Sqlite.Services;

public class SqliteEventBusDbContextSetup : IDbContextSetup
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventEntityTypeConfiguration());
    }
}