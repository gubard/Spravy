using Spravy.PasswordGenerator.Domain.Enums;

namespace Spravy.Ui.Helpers;

public static class UiHelper
{
    public const string ToDoItemEntityNotifyDataFormat = "to-do-item-entity-notify";
    public static bool IsDrag;
    public static object? DragData;
    public static Control? DragControl;
    public static Panel? DragPanel;

    public static readonly IEnumerable<string> Icons =
    [
        string.Empty,
        "mdi-android",
        "mdi-spray-bottle",
        "mdi-water",
        "mdi-chess-queen",
        "mdi-gymnastics",
        "mdi-hair-dryer",
        "mdi-shoe-sneaker",
        "mdi-tshirt-crew",
        "mdi-hand-wash",
        "mdi-bowl",
        "mdi-food-drumstick",
        "mdi-weather-partly-cloudy",
        "mdi-record-rec",
        "mdi-food-steak",
        "mdi-lightbulb-on",
        "mdi-hvac",
        "mdi-radiator",
        "mdi-air-humidifier",
        "mdi-window-open-variant",
        "mdi-timer",
        "mdi-scale-balance",
        "mdi-water-boiler",
        "mdi-fan",
        "mdi-teddy-bear",
        "mdi-calendar-today",
        "mdi-food-variant-off",
        "mdi-kettle-steam",
        "mdi-rewind-60",
        "mdi-fruit-citrus",
        "mdi-heart",
        "mdi-chat",
        "mdi-toilet",
        "mdi-trash-can",
        "mdi-head",
        "mdi-shower-head",
        "mdi-water-thermometer",
        "mdi-egg",
        "mdi-basket",
        "mdi-pot",
        "mdi-pot-steam",
        "mdi-cheese",
        "mdi-toothbrush",
        "mdi-toothbrush-electric",
        "mdi-coffee",
        "mdi-bed-empty",
        "mdi-desktop-classic",
        "mdi-tea",
        "mdi-sprout",
        "mdi-dog-service",
        "mdi-chef-hat",
        "mdi-stove",
        "mdi-gas-burner",
        "mdi-folder-home",
        "mdi-electric-switch",
        "mdi-dishwasher",
        "mdi-silverware-clean",
        "mdi-wiper",
        "mdi-countertop",
        "mdi-email",
        "mdi-knife",
        "mdi-mouse",
        "mdi-lipstick",
        "mdi-content-cut",
        "mdi-face-man-shimmer",
        "mdi-lan-check",
        "mdi-penguin",
    ];

    public static readonly ICommand CopyLogin;
    public static readonly ICommand AddChild;
    public static readonly ICommand MainSplitViewModelInitialized;
    public static readonly ICommand UpdateEmail;
    public static readonly ICommand NavigateToPolicy;
    public static readonly ICommand VerificationEmail;
    public static readonly ICommand Back;
    public static readonly ICommand CopyToClipboard;
    public static readonly ICommand NavigateToToDoItem;
    public static readonly ICommand Complete;
    public static readonly ICommand RemoveFromFavorite;
    public static readonly ICommand AddToFavorite;
    public static readonly ICommand SendNewVerificationCode;
    public static readonly ICommand NavigateToRootToDoItems;
    public static readonly ICommand NavigateToTodayToDoItems;
    public static readonly ICommand NavigateToSearchToDoItems;
    public static readonly ICommand NavigateToPasswordGenerator;
    public static readonly ICommand NavigateToTimers;
    public static readonly ICommand NavigateToEmailOrLoginInput;
    public static readonly ICommand NavigateToCreateUser;
    public static readonly ICommand NavigateToSetting;
    public static readonly ICommand SwitchPane;
    public static readonly ICommand AddTimer;
    public static readonly ICommand DeleteTimer;
    public static readonly ICommand Logout;
    public static readonly ICommand RefreshCurrentView;
    public static readonly ICommand SetToDoItemDescription;
    public static readonly ICommand AddPasswordItem;
    public static readonly ICommand ShowPasswordItemSetting;
    public static readonly ICommand ForgotPassword;
    public static readonly ICommand NavigateToActiveToDoItem;
    public static readonly ICommand CreateUserViewEnter;
    public static readonly ICommand CreateUser;
    public static readonly ICommand LoginViewInitialized;
    public static readonly ICommand MainViewInitialized;
    public static readonly ICommand LoginViewEnter;
    public static readonly ICommand Login;
    public static readonly ICommand GeneratePassword;
    public static readonly ICommand DeletePasswordItem;
    public static readonly SpravyCommandNotify NavigateToCurrentToDoItem;
    public static readonly SpravyCommandNotify ShowSetting;
    public static readonly SpravyCommandNotify RemoveFromBookmark;
    public static readonly ReadOnlyMemory<SpravyCommandNotify> ToDoItemCommands;

    public static ReadOnlyMemory<SortByToDoItem> SortByToDoItems = new(
        [
            SortByToDoItem.Index,
            SortByToDoItem.Name,
            SortByToDoItem.DueDate,
        ]
    );

    public static ReadOnlyMemory<ToDoItemStatus> ToDoItemStatuses = new(
        [
            ToDoItemStatus.Miss,
            ToDoItemStatus.ReadyForComplete,
            ToDoItemStatus.Planned,
            ToDoItemStatus.ComingSoon,
            ToDoItemStatus.Completed,
        ]
    );

    public static ReadOnlyMemory<ToDoItemType> ToDoItemTypes = new(
        [
            ToDoItemType.Value,
            ToDoItemType.Circle,
            ToDoItemType.Group,
            ToDoItemType.Periodicity,
            ToDoItemType.Reference,
            ToDoItemType.Planned,
            ToDoItemType.Step,
            ToDoItemType.PeriodicityOffset,
        ]
    );

    public static ReadOnlyMemory<DayOfWeek> DayOfWeeks = new(
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
        ]
    );

    public static ReadOnlyMemory<GroupBy> GroupBys = new([GroupBy.None, GroupBy.ByStatus, GroupBy.ByType,]);

    public static ReadOnlyMemory<DescriptionType> DescriptionTypes =
        new([DescriptionType.PlainText, DescriptionType.Markdown,]);

    public static ReadOnlyMemory<ToDoItemChildrenType> ToDoItemChildrenTypes =
        new([ToDoItemChildrenType.RequireCompletion, ToDoItemChildrenType.IgnoreCompletion,]);

    public static ReadOnlyMemory<TypeOfPeriodicity> TypeOfPeriodicitys = new(
        [
            TypeOfPeriodicity.Daily,
            TypeOfPeriodicity.Weekly,
            TypeOfPeriodicity.Monthly,
            TypeOfPeriodicity.Annually,
        ]
    );
    
    public static ReadOnlyMemory<PasswordItemType> PasswordItemTypes = new(
        [
            PasswordItemType.Value,
            PasswordItemType.Group,
        ]
    );

    public static ReadOnlyMemory<ThemeType> ThemeTypes = new([ThemeType.Default, ThemeType.Light, ThemeType.Dark,]);

    static UiHelper()
    {
        var commands = DiHelper.ServiceFactory.CreateService<SpravyCommandService>();
        var commandsNotify = DiHelper.ServiceFactory.CreateService<SpravyCommandNotifyService>();
        NavigateToToDoItem = commands.NavigateToToDoItem.Command;
        Complete = commands.Complete.Command;
        RemoveFromFavorite = commands.RemoveFromFavorite.Command;
        AddToFavorite = commands.AddToFavorite.Command;
        AddChild = commands.AddChild.Command;
        SendNewVerificationCode = commands.SendNewVerificationCode.Command;
        NavigateToCurrentToDoItem = commandsNotify.NavigateToCurrentToDoItem;
        Back = commands.Back.Command;
        SwitchPane = commands.SwitchPane.Command;
        Logout = commands.Logout.Command;
        RefreshCurrentView = commands.RefreshCurrentView.Command;
        SetToDoItemDescription = commands.SetToDoItemDescription.Command;
        AddPasswordItem = commands.AddPasswordItem.Command;
        ShowPasswordItemSetting = commands.ShowPasswordItemSetting.Command;
        NavigateToActiveToDoItem = commands.NavigateToActiveToDoItem.Command;
        ForgotPassword = commands.ForgotPassword.Command;
        CreateUserViewEnter = commands.CreateUserViewEnter.Command;
        CreateUser = commands.CreateUser.Command;
        LoginViewInitialized = commands.LoginViewInitialized.Command;
        LoginViewEnter = commands.LoginViewEnter.Command;
        Login = commands.Login.Command;
        GeneratePassword = commands.GeneratePassword.Command;
        DeletePasswordItem = commands.DeletePasswordItem.Command;
        NavigateToRootToDoItems = commands.NavigateToRootToDoItems.Command;
        NavigateToTodayToDoItems = commands.NavigateToTodayToDoItems.Command;
        NavigateToSearchToDoItems = commands.NavigateToSearchToDoItems.Command;
        NavigateToPasswordGenerator = commands.NavigateToPasswordGenerator.Command;
        NavigateToSetting = commands.NavigateToSetting.Command;
        NavigateToPolicy = commands.NavigateToPolicy.Command;
        NavigateToCreateUser = commands.NavigateToCreateUser.Command;
        NavigateToEmailOrLoginInput = commands.NavigateToEmailOrLoginInput.Command;
        CopyToClipboard = commands.CopyToClipboard.Command;
        UpdateEmail = commands.UpdateEmail.Command;
        VerificationEmail = commands.VerificationEmail.Command;
        MainSplitViewModelInitialized = commands.MainSplitViewModelInitialized.Command;
        NavigateToTimers = commands.NavigateToTimers.Command;
        AddTimer = commands.AddTimer.Command;
        DeleteTimer = commands.DeleteTimer.Command;
        MainViewInitialized = commands.MainViewInitialized.Command;
        CopyLogin = commands.CopyLogin.Command;
        ToDoItemCommands = commandsNotify.ToDoItemCommands;
        ShowSetting = commandsNotify.ShowSetting;
        RemoveFromBookmark = commandsNotify.RemoveFromBookmark;
    }

    public static ReadOnlyMemory<object> GetEnumValues(Type type)
    {
        if (typeof(ToDoItemStatus) == type)
        {
            return new(ToDoItemStatuses.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ToDoItemType) == type)
        {
            return new(ToDoItemTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(DayOfWeek) == type)
        {
            return new(DayOfWeeks.ToArray().OfType<object>().ToArray());
        }

        if (typeof(GroupBy) == type)
        {
            return new(GroupBys.ToArray().OfType<object>().ToArray());
        }

        if (typeof(DescriptionType) == type)
        {
            return new(DescriptionTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ToDoItemChildrenType) == type)
        {
            return new(ToDoItemChildrenTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(ThemeType) == type)
        {
            return new(ThemeTypes.ToArray().OfType<object>().ToArray());
        }

        if (typeof(TypeOfPeriodicity) == type)
        {
            return new(TypeOfPeriodicitys.ToArray().OfType<object>().ToArray());
        }

        if (typeof(SortByToDoItem) == type)
        {
            return new(SortByToDoItems.ToArray().OfType<object>().ToArray());
        }

        if (typeof(PasswordItemType) == type)
        {
            return new(PasswordItemTypes.ToArray().OfType<object>().ToArray());
        }

        throw new ArgumentOutOfRangeException(type.ToString());
    }
}