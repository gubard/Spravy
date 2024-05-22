namespace Spravy.Db.Sqlite.Services;

public class SqliteDbContextSetup : IDbContextSetup
{
    private readonly ReadOnlyMemory<StorageEntityTypeConfiguration> configurations;
    private readonly FileInfo dataBaseFile;

    public SqliteDbContextSetup(
        ReadOnlyMemory<StorageEntityTypeConfiguration> configurations,
        FileInfo dataBaseFile,
        bool autoCreateDataBase
    )
    {
        this.configurations = configurations;
        this.dataBaseFile = dataBaseFile;
        AutoCreateDataBase = autoCreateDataBase;
    }

    public bool AutoCreateDataBase { get; }

    public bool DataBaseCreated
    {
        get => dataBaseFile.Exists;
    }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var configuration in configurations.Span)
        {
            modelBuilder.ApplyConfiguration(configuration);
        }
    }

    public void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (builder.IsConfigured)
        {
            return;
        }

        if (AutoCreateDataBase)
        {
            if (dataBaseFile is { Exists: false, Directory.Exists: false, })
            {
                dataBaseFile.Directory.Create();
            }
        }

        builder.UseSqlite(dataBaseFile.ToSqliteConnectionString());
    }
}