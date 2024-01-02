namespace Spravy.Db.Models;

public class StorageEntity
{
    public string Id { get; set; } = string.Empty;
    public byte[] Value { get; set; } = Array.Empty<byte>();
}