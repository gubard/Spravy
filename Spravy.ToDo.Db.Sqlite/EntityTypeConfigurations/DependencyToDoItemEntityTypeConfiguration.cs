using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.ToDo.Db.Models;

namespace Spravy.ToDo.Db.Sqlite.EntityTypeConfigurations;

public class DependencyToDoItemEntityTypeConfiguration : IEntityTypeConfiguration<DependencyToDoItemEntity>
{
    public void Configure(EntityTypeBuilder<DependencyToDoItemEntity> builder)
    {
        builder.ToTable("DependencyToDoItem");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.ToDoItem).WithMany().HasForeignKey(x => x.ToDoItemId);
        builder.HasOne(x => x.DependencyToDoItem).WithMany().HasForeignKey(x => x.DependencyToDoItemId);
    }
}