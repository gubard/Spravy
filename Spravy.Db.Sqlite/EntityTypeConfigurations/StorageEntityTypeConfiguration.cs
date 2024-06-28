namespace Spravy.Db.Sqlite.EntityTypeConfigurations;

public class StorageEntityTypeConfiguration : IEntityTypeConfiguration<StorageEntity>
{
    public void Configure(EntityTypeBuilder<StorageEntity> builder)
    {
        builder.ToTable("Storage");
        builder.HasKey(x => x.Id);
    }
}
