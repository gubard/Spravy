using Microsoft.EntityFrameworkCore;
using Spravy.Db.Core.Interfaces;
using Spravy.ToDo.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.ToDo.Db.Sqlite.Services;

public class SqliteToDoDbContextSetup : IDbContextSetup
{
    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ToDoItemEntityTypeConfiguration());
    }
}