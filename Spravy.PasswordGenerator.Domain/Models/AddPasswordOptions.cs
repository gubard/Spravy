namespace Spravy.PasswordGenerator.Domain.Models;

public readonly struct AddPasswordOptions
{
    public AddPasswordOptions(string name, string key, string availableCharacters, ushort length, string regex)
    {
        Name = name;
        Key = key;
        AvailableCharacters = availableCharacters;
        Length = length;
        Regex = regex;
    }

    public string Name { get; }
    public string Key { get; }
    public string AvailableCharacters { get; }
    public ushort Length { get; }
    public string Regex { get; }
}