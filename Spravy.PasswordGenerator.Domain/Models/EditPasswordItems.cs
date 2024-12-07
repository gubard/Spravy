using Spravy.PasswordGenerator.Domain.Enums;

namespace Spravy.PasswordGenerator.Domain.Models;

public readonly struct EditPasswordItems
{
    public EditPasswordItems(
        ReadOnlyMemory<Guid> ids,
        EditPropertyValue<string> name,
        EditPropertyValue<string> login,
        EditPropertyValue<string> key,
        EditPropertyValue<string> customAvailableCharacters,
        EditPropertyValue<string> regex,
        EditPropertyValue<ushort> length,
        EditPropertyValue<bool> isAvailableUpperLatin,
        EditPropertyValue<bool> isAvailableLowerLatin,
        EditPropertyValue<bool> isAvailableNumber,
        EditPropertyValue<bool> isAvailableSpecialSymbols,
        EditPropertyValue<PasswordItemType> type
    )
    {
        Ids = ids;
        Name = name;
        Login = login;
        Key = key;
        CustomAvailableCharacters = customAvailableCharacters;
        Regex = regex;
        Length = length;
        IsAvailableUpperLatin = isAvailableUpperLatin;
        IsAvailableLowerLatin = isAvailableLowerLatin;
        IsAvailableNumber = isAvailableNumber;
        IsAvailableSpecialSymbols = isAvailableSpecialSymbols;
        Type = type;
    }

    public ReadOnlyMemory<Guid> Ids { get; }
    public EditPropertyValue<string> Name { get; }
    public EditPropertyValue<string> Login { get; }
    public EditPropertyValue<string> Key { get; }
    public EditPropertyValue<string> CustomAvailableCharacters { get; }
    public EditPropertyValue<string> Regex { get; }
    public EditPropertyValue<ushort> Length { get; }
    public EditPropertyValue<bool> IsAvailableUpperLatin { get; }
    public EditPropertyValue<bool> IsAvailableLowerLatin { get; }
    public EditPropertyValue<bool> IsAvailableNumber { get; }
    public EditPropertyValue<bool> IsAvailableSpecialSymbols { get; }
    public EditPropertyValue<PasswordItemType> Type { get; }

    public EditPasswordItems SetIds(ReadOnlyMemory<Guid> ids)
    {
        return new(
            ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetName(EditPropertyValue<string> name)
    {
        return new(
            Ids,
            name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetLogin(EditPropertyValue<string> login)
    {
        return new(
            Ids,
            Name,
            login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetKey(EditPropertyValue<string> key)
    {
        return new(
            Ids,
            Name,
            Login,
            key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetCustomAvailableCharacters(EditPropertyValue<string> customAvailableCharacters)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            customAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetRegex(EditPropertyValue<string> regex)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetLength(EditPropertyValue<ushort> length)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetIsAvailableUpperLatin(EditPropertyValue<bool> isAvailableUpperLatin)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            isAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetIsAvailableLowerLatin(EditPropertyValue<bool> isAvailableLowerLatin)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            isAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetIsAvailableNumber(EditPropertyValue<bool> isAvailableNumber)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            isAvailableNumber,
            IsAvailableSpecialSymbols,
            Type
        );
    }

    public EditPasswordItems SetIsAvailableSpecialSymbols(EditPropertyValue<bool> isAvailableSpecialSymbols)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            isAvailableSpecialSymbols,
            Type
        );
    }
    
    public EditPasswordItems SetType(EditPropertyValue<PasswordItemType> type)
    {
        return new(
            Ids,
            Name,
            Login,
            Key,
            CustomAvailableCharacters,
            Regex,
            Length,
            IsAvailableUpperLatin,
            IsAvailableLowerLatin,
            IsAvailableNumber,
            IsAvailableSpecialSymbols,
            type
        );
    }
}