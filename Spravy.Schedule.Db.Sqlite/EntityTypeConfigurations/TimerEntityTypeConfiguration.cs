using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spravy.Schedule.Db.Models;

namespace Spravy.Schedule.Db.Sqlite.EntityTypeConfigurations;

public class TimerEntityTypeConfiguration : IEntityTypeConfiguration<TimerEntity>
{
    public void Configure(EntityTypeBuilder<TimerEntity> builder)
    {
        builder.ToTable("Timer");
        builder.HasKey(x => x.Id);
    }
}