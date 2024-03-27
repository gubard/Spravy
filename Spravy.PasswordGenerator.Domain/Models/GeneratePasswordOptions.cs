namespace Spravy.PasswordGenerator.Domain.Models;

public readonly struct GeneratePasswordOptions
{
    public GeneratePasswordOptions(string key, string availableCharacters, ushort length, string regex)
    {
        Key = key;
        AvailableCharacters = availableCharacters;
        Length = length;
        Regex = regex;
    }

    public string Key { get; }
    public string AvailableCharacters { get; }
    public ushort Length { get; }
    public string Regex { get; }
}