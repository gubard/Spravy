using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Db.Models;

namespace Spravy.Db.Sqlite.EntityTypeConfigurations;

public class ToDoItemValueEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItemValueEntity>
{
    public void Configure(EntityTypeBuilder<ToDoItemValueEntity> builder)
    {
        builder.ToTable("ToDoItemValue");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Item).WithOne(x => x.Value).HasForeignKey<ToDoItemValueEntity>(x => x.ItemId);
    }
}