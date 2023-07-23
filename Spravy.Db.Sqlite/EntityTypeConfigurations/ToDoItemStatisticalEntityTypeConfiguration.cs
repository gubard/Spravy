using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Db.Models;

namespace Spravy.Db.Sqlite.EntityTypeConfigurations;

public class ToDoItemStatisticalEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItemStatisticalEntity>
{
    public void Configure(EntityTypeBuilder<ToDoItemStatisticalEntity> builder)
    {
        builder.ToTable("ToDoItemStatistical");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Item).WithOne(x => x.Statistical).HasForeignKey<ToDoItemValueEntity>(x => x.ItemId);
    }
}