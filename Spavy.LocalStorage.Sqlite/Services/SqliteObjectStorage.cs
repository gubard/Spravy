using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spavy.LocalStorage.Sqlite.Services;

public class SqliteObjectStorage : IObjectStorage
{
    private readonly ISerializer serializer;
    private readonly FileInfo file;
    private readonly string connectionString;

    public SqliteObjectStorage(ISerializer serializer, FileInfo file)
    {
        this.serializer = serializer;
        this.file = file;
        connectionString = $"DataSource={file}";
    }

    private void Init()
    {
        if (file.Exists)
        {
            return;
        }

        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE Storage (Id TEXT PRIMARY KEY, Value BLOB);";
            command.ExecuteNonQuery();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();

            throw;
        }
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> IsExistsAsync(string id, CancellationToken ct)
    {
        return IsExistsCore(id, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteAsync(string id, CancellationToken ct)
    {
        return DeleteCore(id, ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> DeleteCore(string id, CancellationToken ct)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Storage WHERE Id = @Id";
            command.Parameters.Add(new("@Id", id));
            await command.ExecuteNonQueryAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }

        return Result.Success;
    }

    private async ValueTask<Result<bool>> IsExistsCore(string id, CancellationToken ct)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Storage WHERE Id = @Id";
            command.Parameters.Add(new("@Id", id));
            var scalar = await command.ExecuteScalarAsync(ct);
            await transaction.CommitAsync(ct);

            if (scalar is long and > 0)
            {
                return true.ToResult();
            }

            return false.ToResult();
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }
    }

    public ConfiguredValueTaskAwaitable<Result<TObject>> GetObjectAsync<TObject>(
        string id,
        CancellationToken ct
    )
        where TObject : notnull
    {
        return GetObjectCore<TObject>(id, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> SaveObjectAsync(
        string id,
        object obj,
        CancellationToken ct
    )
    {
        return SaveObjectCore(id, obj, ct).ConfigureAwait(false);
    }

    public async ValueTask<Result> SaveObjectCore(string id, object obj, CancellationToken ct)
    {
        await using var stream = new MemoryStream();
        await serializer.SerializeAsync(obj, stream, ct);
        stream.Position = 0;
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Storage WHERE Id = @Id;";
            command.Parameters.Add(new("@Id", id));
            var scalar = await command.ExecuteScalarAsync(ct);

            if (scalar is long and > 0)
            {
                command.CommandText = "UPDATE Storage SET Value = @Value WHERE Id = @Id;";
                command.Parameters.Add(new("@Value", stream.ToArray()));
            }
            else
            {
                command.CommandText = "INSERT INTO Storage (Id, Value) VALUES (@Id, @Value);";
                command.Parameters.Add(new("@Value", stream.ToArray()));
            }

            await command.ExecuteNonQueryAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }

        return Result.Success;
    }

    private async ValueTask<Result<TObject>> GetObjectCore<TObject>(string id, CancellationToken ct)
        where TObject : notnull
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT Value FROM Storage WHERE Id = @Id;";
            command.Parameters.Add(new("@Id", id));
            var scalar = await command.ExecuteScalarAsync(ct);
            await transaction.CommitAsync(ct);
            var array = (byte[])scalar.ThrowIfNull();
            await using var stream = new MemoryStream(array);

            return await serializer.DeserializeAsync<TObject>(stream, ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);

            throw;
        }
    }
}
