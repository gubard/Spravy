using Microsoft.EntityFrameworkCore;
using Spravy.Db.Interfaces;
using Spravy.Picture.Db.Sqlite.EntityTypeConfigurations;

namespace Spravy.Picture.Db.Sqlite.Services;

public class SqlitePictureDbContextSetup : IDbContextSetup
{
    public bool AutoCreateDataBase => false;

    public bool DataBaseCreated => true;

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PictureItemEntityTypeConfiguration());
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
    }
}