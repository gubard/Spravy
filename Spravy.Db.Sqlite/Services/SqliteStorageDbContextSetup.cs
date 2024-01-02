using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.Db.Sqlite.Services;

public class SqliteStorageDbContextSetup : IDbContextSetup
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StorageEntityTypeConfiguration());
    }
}