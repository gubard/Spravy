namespace Spravy.PasswordGenerator.Domain.Models;

public readonly struct PasswordItem
{
    public PasswordItem(Guid id, string name, string key, string availableCharacters, ushort length, string regex)
    {
        Id = id;
        Name = name;
        Key = key;
        AvailableCharacters = availableCharacters;
        Length = length;
        Regex = regex;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Key { get; }
    public string AvailableCharacters { get; }
    public ushort Length { get; }
    public string Regex { get; }
}