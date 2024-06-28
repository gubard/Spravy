using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.ToDo.Db.Models;

namespace Spravy.ToDo.Db.Sqlite.EntityTypeConfigurations;

public class ToDoItemEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItemEntity>
{
    public void Configure(EntityTypeBuilder<ToDoItemEntity> builder)
    {
        builder.ToTable("ToDoItem");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);
        builder.HasOne(x => x.Reference).WithMany().HasForeignKey(x => x.ReferenceId);
        builder.Property(x => x.DaysOfMonth).HasDefaultValue("1");
        builder.Property(x => x.DaysOfWeek).HasDefaultValue(DayOfWeek.Monday.ToString());
        builder.Property(x => x.DaysOfYear).HasDefaultValue("1.1");
        builder.Property(x => x.Link).HasDefaultValue("");
        builder.Property(x => x.CreatedDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
