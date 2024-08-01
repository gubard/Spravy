using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;

namespace Spravy.Ui.Mappers;

[Mapper(PreferParameterlessConstructors = false)]
public static partial class UiMapper
{
    public static partial CreateUserOptions ToCreateUserOptions(this CreateUserViewModel value);

    public static partial User ToUser(this CreateUserViewModel value);

    public static partial User ToUser(this LoginViewModel value);

    public static partial AddPasswordOptions ToAddPasswordOptions(
        this AddPasswordItemViewModel value
    );

    public static partial ResetToDoItemOptions ToResetToDoItemOptions(
        this ResetToDoItemViewModel value
    );

    public static ThemeType ToThemeType(this ThemeVariant value)
    {
        var key = value.Key.ThrowIfIsNotCast<string>();

        return key switch
        {
            nameof(ThemeVariant.Default) => ThemeType.Default,
            nameof(ThemeVariant.Dark) => ThemeType.Dark,
            nameof(ThemeVariant.Light) => ThemeType.Light,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static ThemeVariant ToThemeVariant(this ThemeType value)
    {
        return value switch
        {
            ThemeType.Default => ThemeVariant.Default,
            ThemeType.Light => ThemeVariant.Light,
            ThemeType.Dark => ThemeVariant.Dark,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public static AddRootToDoItemOptions ToAddRootToDoItemOptions(
        this AddRootToDoItemViewModel value
    )
    {
        return new(
            value.ToDoItemContent.Name,
            value.ToDoItemContent.Type,
            value.ToDoItemContent.Link.ToOptionUri(),
            value.DescriptionContent.Description,
            value.DescriptionContent.Type
        );
    }
}
