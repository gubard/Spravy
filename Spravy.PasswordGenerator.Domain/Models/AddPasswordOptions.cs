namespace Spravy.PasswordGenerator.Domain.Models;

public readonly struct AddPasswordOptions
{
    public AddPasswordOptions(
        string name,
        string key,
        ushort length,
        string regex,
        bool isAvailableLowerLatin,
        bool isAvailableUpperLatin,
        bool isAvailableNumber,
        bool isAvailableSpecialSymbols,
        string customAvailableCharacters
    )
    {
        Name = name;
        Key = key;
        Length = length;
        Regex = regex;
        IsAvailableLowerLatin = isAvailableLowerLatin;
        IsAvailableUpperLatin = isAvailableUpperLatin;
        IsAvailableNumber = isAvailableNumber;
        IsAvailableSpecialSymbols = isAvailableSpecialSymbols;
        CustomAvailableCharacters = customAvailableCharacters;
    }

    public string Name { get; }
    public string Key { get; }
    public bool IsAvailableUpperLatin { get; }
    public bool IsAvailableLowerLatin { get; }
    public bool IsAvailableNumber { get; }
    public bool IsAvailableSpecialSymbols { get; }
    public string CustomAvailableCharacters { get; }
    public ushort Length { get; }
    public string Regex { get; }
}