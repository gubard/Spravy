using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Db.Models;

namespace Spravy.Db.Sqlite.EntityTypeConfigurations;

public class StorageEntityTypeConfiguration : IEntityTypeConfiguration<StorageEntity>
{
    public void Configure(EntityTypeBuilder<StorageEntity> builder)
    {
        builder.ToTable("Storage");
        builder.HasKey(x => x.Id);
    }
}