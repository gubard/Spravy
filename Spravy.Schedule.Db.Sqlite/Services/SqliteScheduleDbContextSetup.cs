using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Schedule.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.Schedule.Db.Sqlite.Services;

public class SqliteScheduleDbContextSetup : IDbContextSetup
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TimerEntityTypeConfiguration());
    }
}