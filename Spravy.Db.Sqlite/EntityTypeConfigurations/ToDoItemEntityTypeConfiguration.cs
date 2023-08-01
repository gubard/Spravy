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
        builder.Property(x => x.DaysOfMonth).HasDefaultValue("1");
        builder.Property(x => x.DaysOfWeek).HasDefaultValue(DayOfWeek.Monday.ToString());
        builder.Property(x => x.DaysOfYear).HasDefaultValue("1.1");
    }
}