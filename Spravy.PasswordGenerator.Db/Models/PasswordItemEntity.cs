namespace Spravy.PasswordGenerator.Db.Models;

public class PasswordItemEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public bool IsAvailableUpperLatin { get; set; } 
    public bool IsAvailableLowerLatin { get; set; }
    public bool IsAvailableNumber { get; set; }
    public bool IsAvailableSpecialSymbols { get; set; } 
    public string CustomAvailableCharacters { get; set; } = string.Empty;
    public ushort Length { get; set; }
    public string Regex { get; set; } = string.Empty;
}