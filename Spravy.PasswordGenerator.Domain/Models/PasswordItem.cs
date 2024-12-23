using Spravy.PasswordGenerator.Domain.Enums;

namespace Spravy.PasswordGenerator.Domain.Models;

public readonly struct PasswordItem
{
    public PasswordItem(
        Guid id,
        string name,
        string key,
        ushort length,
        string regex,
        bool isAvailableUpperLatin,
        bool isAvailableLowerLatin,
        bool isAvailableSpecialSymbols,
        bool isAvailableNumber,
        string customAvailableCharacters,
        string login,
        PasswordItemType type,
        uint orderIndex
    )
    {
        Id = id;
        Name = name;
        Key = key;
        Length = length;
        Regex = regex;
        IsAvailableUpperLatin = isAvailableUpperLatin;
        IsAvailableLowerLatin = isAvailableLowerLatin;
        IsAvailableSpecialSymbols = isAvailableSpecialSymbols;
        IsAvailableNumber = isAvailableNumber;
        CustomAvailableCharacters = customAvailableCharacters;
        Login = login;
        Type = type;
        OrderIndex = orderIndex;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Key { get; }
    public bool IsAvailableUpperLatin { get; }
    public bool IsAvailableLowerLatin { get; }
    public bool IsAvailableNumber { get; }
    public bool IsAvailableSpecialSymbols { get; }
    public string CustomAvailableCharacters { get; }
    public ushort Length { get; }
    public string Regex { get; }
    public string Login { get; }
    public PasswordItemType Type { get; }
    public uint OrderIndex { get; }
}