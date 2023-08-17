using Microsoft.EntityFrameworkCore;
using Spravy.ToDo.Db.Interfaces;
using Spravy.ToDo.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.ToDo.Db.Sqlite.Services;

public class SqliteDbContextSetup : IDbContextSetup
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ToDoItemEntityTypeConfiguration());
    }
}