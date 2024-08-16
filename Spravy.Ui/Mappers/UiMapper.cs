using Riok.Mapperly.Abstractions;
using Spravy.Core.Mappers;
using Spravy.Schedule.Domain.Models;
using Spravy.Ui.Features.Schedule.Models;

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

    public static partial TimerItemNotify ToTimerItemNotify(this TimerItem value);

    public static AddPasswordOptions ToAddPasswordOptions(this AddPasswordItemViewModel value)
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
            value.CustomAvailableCharacters
        );
    }

    public static ResetToDoItemOptions ToResetToDoItemOptions(this ResetToDoItemViewModel value)
    {
        return new(
            value.Item.Id,
            value.IsCompleteChildrenTask,
            value.IsMoveCircleOrderIndex,
            value.IsOnlyCompletedTasks,
            value.IsCompleteCurrentTask
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
            value.DescriptionContent.DescriptionType
        );
    }
}
