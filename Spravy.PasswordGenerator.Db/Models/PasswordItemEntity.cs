namespace Spravy.PasswordGenerator.Db.Models;

public class PasswordItemEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string AvailableCharacters { get; set; } = string.Empty;
    public ushort Length { get; set; }
    public string Regex { get; set; } = string.Empty;
}