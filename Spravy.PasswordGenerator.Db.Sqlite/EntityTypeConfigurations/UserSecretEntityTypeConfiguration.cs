using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.PasswordGenerator.Db.Models;

namespace Spravy.PasswordGenerator.Db.Sqlite.EntityTypeConfigurations;

public class UserSecretEntityTypeConfiguration : IEntityTypeConfiguration<UserSecretEntity>
{
    public void Configure(EntityTypeBuilder<UserSecretEntity> builder)
    {
        builder.ToTable("UserSecrets");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId).IsUnique();
        ;
    }
}
