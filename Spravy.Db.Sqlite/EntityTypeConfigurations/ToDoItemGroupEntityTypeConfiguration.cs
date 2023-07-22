using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Db.Models;

namespace Spravy.Db.Sqlite.EntityTypeConfigurations;

public class ToDoItemGroupEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItemGroupEntity>
{
    public void Configure(EntityTypeBuilder<ToDoItemGroupEntity> builder)
    {
        builder.ToTable("ToDoItemGroup");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Item).WithOne(x => x.Group).HasForeignKey<ToDoItemGroupEntity>(x => x.ItemId);
    }
}