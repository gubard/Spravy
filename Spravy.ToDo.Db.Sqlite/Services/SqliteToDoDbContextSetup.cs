using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.ToDo.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.ToDo.Db.Sqlite.Services;

public class SqliteToDoDbContextSetup : IDbContextSetup
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
        modelBuilder.ApplyConfiguration(new ToDoItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new DependencyToDoItemEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}