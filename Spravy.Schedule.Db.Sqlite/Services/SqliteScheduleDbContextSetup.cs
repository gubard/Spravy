using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Schedule.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.Schedule.Db.Sqlite.Services;

public class SqliteScheduleDbContextSetup : IDbContextSetup
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
        modelBuilder.ApplyConfiguration(new TimerEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder) { }
}
