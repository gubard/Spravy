namespace Spravy.Db.Interfaces;

public interface IDbContextSetup
{
    bool AutoCreateDataBase { get; }
    bool DataBaseCreated { get; }
    void OnModelCreating(ModelBuilder modelBuilder);
    void OnConfiguring(DbContextOptionsBuilder builder);
}