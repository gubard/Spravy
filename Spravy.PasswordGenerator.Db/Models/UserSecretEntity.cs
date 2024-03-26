namespace Spravy.PasswordGenerator.Db.Models;

public class UserSecretEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public byte[] Secret { get; set; } = Array.Empty<byte>();
}