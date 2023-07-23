using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Db.Models;

namespace Spravy.Db.Sqlite.EntityTypeConfigurations;

public class ToDoItemEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItemEntity>
{
    public void Configure(EntityTypeBuilder<ToDoItemEntity> builder)
    {
        builder.ToTable("ToDoItem");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);
        builder.HasOne(x => x.Value).WithOne(x => x.Item).HasForeignKey<ToDoItemEntity>(x => x.ValueId);
        builder.HasOne(x => x.Group).WithOne(x => x.Item).HasForeignKey<ToDoItemEntity>(x => x.GroupId);
        builder.HasOne(x => x.Statistical).WithOne(x => x.Item).HasForeignKey<ToDoItemEntity>(x => x.StatisticalId);
    }
}