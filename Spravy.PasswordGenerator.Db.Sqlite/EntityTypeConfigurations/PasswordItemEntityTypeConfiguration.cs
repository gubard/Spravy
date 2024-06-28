using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.PasswordGenerator.Db.Models;

namespace Spravy.PasswordGenerator.Db.Sqlite.EntityTypeConfigurations;

public class PasswordItemEntityTypeConfiguration : IEntityTypeConfiguration<PasswordItemEntity>
{
    public void Configure(EntityTypeBuilder<PasswordItemEntity> builder)
    {
        builder.ToTable("PasswordItems");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.Key).IsUnique();
    }
}
