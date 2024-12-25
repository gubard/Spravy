using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Picture.Db.Models;

namespace Spravy.Picture.Db.Sqlite.EntityTypeConfigurations;

public class PictureItemEntityTypeConfiguration : IEntityTypeConfiguration<PictureItemEntity>
{
    public void Configure(EntityTypeBuilder<PictureItemEntity> builder)
    {
        builder.ToTable("Picture");
        builder.HasKey(x => x.Id);
    }
}