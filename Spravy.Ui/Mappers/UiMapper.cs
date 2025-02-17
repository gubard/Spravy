using Riok.Mapperly.Abstractions;

namespace Spravy.Ui.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class UiMapper
{
    public static CreateUserOptions ToCreateUserOptions(this CreateUserViewModel value)
    {
        return new(value.Login, value.Password, value.Email);
    }

    public static User ToUser(this CreateUserViewModel value)
    {
        return new(value.Login, value.Password);
    }

    public static User ToUser(this LoginViewModel value)
    {
        return new(value.Login, value.Password);
    }

    public static TimerItemNotify ToTimerItemNotify(this TimerItem value)
    {
        return new(value.Id)
        {
            Name = value.Name,
            DueDateTime = value.DueDateTime,
        };
    }

    public static AddPasswordOptions ToAddPasswordOptions(this EditPasswordItemViewModel value)
    {
        return new(
            value.Name,
            value.Key,
            value.Length,
            value.Regex,
            value.IsAvailableLowerLatin,
            value.IsAvailableUpperLatin,
            value.IsAvailableNumber,
            value.IsAvailableSpecialSymbols,
            value.CustomAvailableCharacters,
            value.Login,
            value.Type
        );
    }

    public static ThemeType ToThemeType(this ThemeVariant value)
    {
        var key = value.Key.ThrowIfIsNotCast<string>();

        return key switch
        {
            nameof(ThemeVariant.Default) => ThemeType.Default,
            nameof(ThemeVariant.Dark) => ThemeType.Dark,
            nameof(ThemeVariant.Light) => ThemeType.Light,
            _ => throw new ArgumentOutOfRangeException(key),
        };
    }

    public static ThemeVariant ToThemeVariant(this ThemeType value)
    {
        return value switch
        {
            ThemeType.Default => ThemeVariant.Default,
            ThemeType.Light => ThemeVariant.Light,
            ThemeType.Dark => ThemeVariant.Dark,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}