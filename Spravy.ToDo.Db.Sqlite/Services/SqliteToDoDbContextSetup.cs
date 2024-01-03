using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.ToDo.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.ToDo.Db.Sqlite.Services;

public class SqliteToDoDbContextSetup : IDbContextSetup
{
    public bool AutoCreateDataBase => false;
    public bool DataBaseCreated => true;
    
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ToDoItemEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}