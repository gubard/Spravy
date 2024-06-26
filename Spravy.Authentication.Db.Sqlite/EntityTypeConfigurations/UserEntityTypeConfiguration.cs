using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Authentication.Db.Models;

namespace Spravy.Authentication.Db.Sqlite.EntityTypeConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("User");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Login).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}
