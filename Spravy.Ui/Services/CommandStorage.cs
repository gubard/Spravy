using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Serilog;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using Spravy.Domain.Extensions;

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    static CommandStorage()
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        objectStorage = kernel.Get<IObjectStorage>();
        mapper = kernel.Get<IMapper>();
        tokenService = kernel.Get<ITokenService>();
        authenticationService = kernel.Get<IAuthenticationService>();
        navigator = kernel.Get<INavigator>();
        openerLink = kernel.Get<IOpenerLink>();
        dialogViewer = kernel.Get<IDialogViewer>();
        mainSplitViewModel = kernel.Get<MainSplitViewModel>();
        toDoService = kernel.Get<IToDoService>();
        SwitchPane = CreateCommand(SwitchPaneAsync, MaterialIconKind.Menu, "Open pane");
        NavigateToToDoItem = CreateCommand<Guid>(NavigateToToDoItemAsync, MaterialIconKind.ListBox, "Open");
        CompleteToDoItem = CreateCommand<ICanComplete>(CompleteToDoItemAsync, MaterialIconKind.Check, "Complete");
        SelectAll = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            SelectAllAsync,
            MaterialIconKind.CheckAll,
            "Select all"
        );
        DeleteToDoItem = CreateCommand<IDeletable>(DeleteSubToDoItemAsync, MaterialIconKind.Delete, "Delete");
        ChangeToActiveDoItem = CreateCommand(ChangeToActiveDoItemAsync, MaterialIconKind.ArrowRight, "Go to active");
        ChangeOrderIndex = CreateCommand<ToDoItemNotify>(
            ChangeOrderIndexAsync,
            MaterialIconKind.ReorderHorizontal,
            "Reorder"
        );
        OpenLink = CreateCommand<ToDoItemNotify>(
            OpenLinkAsync,
            MaterialIconKind.Link,
            "Reorder"
        );
        RemoveToDoItemFromFavorite = CreateCommand<Guid>(
            RemoveFavoriteToDoItemAsync,
            MaterialIconKind.Link,
            "Remove from favorite"
        );
        AddToDoItemToFavorite = CreateCommand<Guid>(
            AddFavoriteToDoItemAsync,
            MaterialIconKind.Link,
            "Add to favorite"
        );
        SetToDoShortItem = CreateCommand<IToDoShortItemProperty>(
            SetToDoShortItemAsync,
            MaterialIconKind.Pencil,
            "Set to do item"
        );
        SetDueDateTime = CreateCommand<IDueDateTimeProperty>(
            SetDueDateTimeAsync,
            MaterialIconKind.Pencil,
            "Set due date time"
        );
        CreateUser = CreateCommand<ICreateUserProperties>(
            CreateUserAsync,
            MaterialIconKind.AccountPlus,
            "Set due date time"
        );
        Back = CreateCommand(BackAsync, MaterialIconKind.ArrowLeft, "Back");
        NavigateTo = CreateCommand<Type>(NavigateToAsync, MaterialIconKind.ArrowLeft, "Navigate to");
        Login = CreateCommand<ILoginProperties>(LoginAsync, MaterialIconKind.Login, "Login");
        Logout = CreateCommand(LogoutAsync, MaterialIconKind.Logout, "Logout");
        SetToDoChildrenType = CreateCommand<IToDoChildrenTypeProperty>(
            SetToDoChildrenTypeAsync,
            MaterialIconKind.Pencil,
            "Set children type"
        );
        SetToDoDueDate = CreateCommand<IToDoDueDateProperty>(
            SetToDoDueDateAsync,
            MaterialIconKind.Pencil,
            "Set due date"
        );
        SetToDoDaysOffset = CreateCommand<IToDoDaysOffsetProperty>(
            SetToDoDaysOffsetAsync,
            MaterialIconKind.Pencil,
            "Set days offset"
        );
        SetToDoMonthsOffset = CreateCommand<IToDoMonthsOffsetProperty>(
            SetToDoMonthsOffsetAsync,
            MaterialIconKind.Pencil,
            "Set months offset"
        );
        SetToDoWeeksOffset = CreateCommand<IToDoWeeksOffsetProperty>(
            SetToDoWeeksOffsetAsync,
            MaterialIconKind.Pencil,
            "Set weeks offset"
        );
        SetToDoYearsOffset = CreateCommand<IToDoYearsOffsetProperty>(
            SetToDoYearsOffsetAsync,
            MaterialIconKind.Pencil,
            "Set years offset"
        );
        SetToDoTypeOfPeriodicity = CreateCommand<IToDoTypeOfPeriodicityProperty>(
            SetToDoTypeOfPeriodicityAsync,
            MaterialIconKind.Pencil,
            "Set type of periodicity"
        );
        SetToDoPeriodicity = CreateCommand<IToDoTypeOfPeriodicityProperty>(
            SetToDoPeriodicityAsync,
            MaterialIconKind.Pencil,
            "Set periodicity"
        );
        AddRootToDoItem = CreateCommand(
            AddRootToDoItemAsync,
            MaterialIconKind.Plus,
            "Add root to do item"
        );
    }

    private static readonly INavigator navigator;
    private static readonly IDialogViewer dialogViewer;
    private static readonly MainSplitViewModel mainSplitViewModel;
    private static readonly IToDoService toDoService;
    private static readonly IOpenerLink openerLink;
    private static readonly IMapper mapper;
    private static readonly IAuthenticationService authenticationService;
    private static readonly ITokenService tokenService;
    private static readonly IObjectStorage objectStorage;

    public static CommandParameters SwitchPane { get; }
    public static ICommand SwitchPaneCommand => SwitchPane.Value.Command;
    public static CommandItem SwitchPaneItem => SwitchPane.Value;

    public static CommandParameters<ICanComplete> CompleteToDoItem { get; }
    public static ICommand CompleteToDoItemCommand => CompleteToDoItem.Value.Command;
    public static CommandItem CompleteToDoItemItem => CompleteToDoItem.Value;

    public static CommandParameters ChangeToActiveDoItem { get; }
    public static ICommand ChangeToActiveDoItemCommand => ChangeToActiveDoItem.Value.Command;
    public static CommandItem ChangeToActiveDoItemItem => ChangeToActiveDoItem.Value;

    public static CommandParameters<ToDoItemNotify> ChangeOrderIndex { get; }
    public static ICommand ChangeOrderIndexCommand => ChangeOrderIndex.Value.Command;
    public static CommandItem ChangeOrderIndexItem => ChangeOrderIndex.Value;

    public static CommandParameters<IDeletable> DeleteToDoItem { get; }
    public static ICommand DeleteToDoItemCommand => DeleteToDoItem.Value.Command;
    public static CommandItem DeleteToDoItemItem => DeleteToDoItem.Value;

    public static CommandParameters<Guid> NavigateToToDoItem { get; }
    public static ICommand NavigateToToDoItemCommand => NavigateToToDoItem.Value.Command;
    public static CommandItem NavigateToToDoItemItem => NavigateToToDoItem.Value;

    public static CommandParameters<ToDoItemNotify> OpenLink { get; }
    public static ICommand OpenLinkCommand => OpenLink.Value.Command;
    public static CommandItem OpenLinkItem => OpenLink.Value;

    public static CommandParameters<Guid> RemoveToDoItemFromFavorite { get; }
    public static ICommand RemoveToDoItemFromFavoriteCommand => RemoveToDoItemFromFavorite.Value.Command;
    public static CommandItem RemoveToDoItemFromFavoriteItem => RemoveToDoItemFromFavorite.Value;

    public static CommandParameters<Guid> AddToDoItemToFavorite { get; }
    public static ICommand AddToDoItemToFavoriteCommand => AddToDoItemToFavorite.Value.Command;
    public static CommandItem AddToDoItemToFavoriteItem => AddToDoItemToFavorite.Value;

    public static CommandParameters<IToDoShortItemProperty> SetToDoShortItem { get; }
    public static ICommand SetToDoShortItemCommand => SetToDoShortItem.Value.Command;
    public static CommandItem SetToDoShortItemItem => SetToDoShortItem.Value;

    public static CommandParameters<IDueDateTimeProperty> SetDueDateTime { get; }
    public static ICommand SetDueDateTimeCommand => SetDueDateTime.Value.Command;
    public static CommandItem SetDueDateTimeItem => SetDueDateTime.Value;

    public static CommandParameters<ICreateUserProperties> CreateUser { get; }
    public static ICommand CreateUserCommand => CreateUser.Value.Command;
    public static CommandItem CreateUserItem => CreateUser.Value;

    public static CommandParameters Back { get; }
    public static ICommand BackCommand => Back.Value.Command;
    public static CommandItem BackItem => Back.Value;

    public static CommandParameters<Type> NavigateTo { get; }
    public static ICommand NavigateToCommand => NavigateTo.Value.Command;
    public static CommandItem NavigateToItem => NavigateTo.Value;

    public static CommandParameters<ILoginProperties> Login { get; }
    public static ICommand LoginCommand => Login.Value.Command;
    public static CommandItem LoginItem => Login.Value;

    public static CommandParameters Logout { get; }
    public static ICommand LogoutCommand => Logout.Value.Command;
    public static CommandItem LogoutItem => Logout.Value;

    public static CommandParameters<IToDoChildrenTypeProperty> SetToDoChildrenType { get; }
    public static ICommand SetToDoChildrenTypeCommand => SetToDoChildrenType.Value.Command;
    public static CommandItem SetToDoChildrenTypeItem => SetToDoChildrenType.Value;

    public static CommandParameters<IToDoDueDateProperty> SetToDoDueDate { get; }
    public static ICommand SetToDoDueDateCommand => SetToDoDueDate.Value.Command;
    public static CommandItem SetToDoDueDateItem => SetToDoDueDate.Value;

    public static CommandParameters<IToDoDaysOffsetProperty> SetToDoDaysOffset { get; }
    public static ICommand SetToDoDaysOffsetCommand => SetToDoDaysOffset.Value.Command;
    public static CommandItem SetToDoDaysOffsetItem => SetToDoDaysOffset.Value;

    public static CommandParameters<IToDoMonthsOffsetProperty> SetToDoMonthsOffset { get; }
    public static ICommand SetToDoMonthsOffsetCommand => SetToDoMonthsOffset.Value.Command;
    public static CommandItem SetToDoMonthsOffsetItem => SetToDoMonthsOffset.Value;

    public static CommandParameters<IToDoWeeksOffsetProperty> SetToDoWeeksOffset { get; }
    public static ICommand SetToDoWeeksOffsetCommand => SetToDoWeeksOffset.Value.Command;
    public static CommandItem SetToDoWeeksOffsetItem => SetToDoWeeksOffset.Value;

    public static CommandParameters<IToDoYearsOffsetProperty> SetToDoYearsOffset { get; }
    public static ICommand SetToDoYearsOffsetCommand => SetToDoYearsOffset.Value.Command;
    public static CommandItem SetToDoYearsOffsetItem => SetToDoYearsOffset.Value;

    public static CommandParameters<IToDoTypeOfPeriodicityProperty> SetToDoTypeOfPeriodicity { get; }
    public static ICommand SetToDoTypeOfPeriodicityCommand => SetToDoTypeOfPeriodicity.Value.Command;
    public static CommandItem SetToDoTypeOfPeriodicityItem => SetToDoTypeOfPeriodicity.Value;

    public static CommandParameters<IToDoTypeOfPeriodicityProperty> SetToDoPeriodicity { get; }
    public static ICommand SetToDoPeriodicityCommand => SetToDoPeriodicity.Value.Command;
    public static CommandItem SetToDoPeriodicityItem => SetToDoPeriodicity.Value;

    public static CommandParameters AddRootToDoItem { get; }
    public static ICommand AddRootToDoItemCommand => AddRootToDoItem.Value.Command;
    public static CommandItem AddRootToDoItemItem => AddRootToDoItem.Value;

    public static CommandParameters<AvaloniaList<Selected<ToDoItemNotify>>> SelectAll { get; }

    private static Task AddRootToDoItemAsync(CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            async view =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var options = mapper.Map<AddRootToDoItemOptions>(view);
                cancellationToken.ThrowIfCancellationRequested();
                await toDoService.AddRootToDoItemAsync(options, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            async _ => await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
            ActionHelper<AddRootToDoItemViewModel>.Empty,
            cancellationToken
        );
    }

    private static async Task SetToDoPeriodicityAsync(
        IToDoTypeOfPeriodicityProperty property,
        CancellationToken cancellationToken
    )
    {
        switch (property.TypeOfPeriodicity)
        {
            case TypeOfPeriodicity.Weekly:
            {
                var periodicity = await toDoService.GetWeeklyPeriodicityAsync(property.Id, cancellationToken)
                    .ConfigureAwait(false);

                await dialogViewer.ShowDayOfWeekSelectorInputDialogAsync(
                        async days =>
                        {
                            await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                            await toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
                                    property.Id,
                                    new WeeklyPeriodicity(days),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                            await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                            await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
                        },
                        viewModel =>
                        {
                            foreach (var item in viewModel.Items)
                            {
                                if (periodicity.Days.Contains(item.DayOfWeek))
                                {
                                    item.IsSelected = true;
                                }
                            }
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            }
            case TypeOfPeriodicity.Monthly:
            {
                var periodicity = await toDoService.GetMonthlyPeriodicityAsync(property.Id, cancellationToken)
                    .ConfigureAwait(false);

                await dialogViewer.ShowDayOfMonthSelectorInputDialogAsync(
                        async days =>
                        {
                            await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                            await toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
                                    property.Id,
                                    new MonthlyPeriodicity(days),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                            await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                            await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
                        },
                        viewModel =>
                        {
                            foreach (var item in viewModel.Items)
                            {
                                if (periodicity.Days.Contains(item.Day))
                                {
                                    item.IsSelected = true;
                                }
                            }
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            }
            case TypeOfPeriodicity.Annually:
            {
                var periodicity = await toDoService.GetAnnuallyPeriodicityAsync(property.Id, cancellationToken)
                    .ConfigureAwait(false);

                await dialogViewer.ShowDayOfYearSelectorInputDialogAsync(
                        async days =>
                        {
                            await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                            await toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
                                    property.Id,
                                    new AnnuallyPeriodicity(days),
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                            await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                            await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
                        },
                        viewModel =>
                        {
                            foreach (var month in viewModel.Items)
                            {
                                foreach (var day in month.Days.Items)
                                {
                                    if (periodicity.Days.Any(x => x.Month == month.Month && x.Day == day.Day))
                                    {
                                        day.IsSelected = true;
                                    }
                                }
                            }
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            }
            case TypeOfPeriodicity.Daily:
                throw new ArgumentOutOfRangeException();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static Task SetToDoTypeOfPeriodicityAsync(
        IToDoTypeOfPeriodicityProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowItemSelectorDialogAsync<TypeOfPeriodicity>(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemTypeOfPeriodicityAsync(property.Id, value, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            calendar =>
            {
                calendar.Items.AddRange(Enum.GetValues<TypeOfPeriodicity>().OfType<object>());
                calendar.SelectedItem = property.TypeOfPeriodicity;
            },
            cancellationToken
        );
    }

    private static Task SetToDoYearsOffsetAsync(IToDoYearsOffsetProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowNumberUInt16InputDialogAsync(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemYearsOffsetAsync(property.Id, value, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            calendar => calendar.Value = property.YearsOffset,
            cancellationToken
        );
    }

    private static Task SetToDoWeeksOffsetAsync(IToDoWeeksOffsetProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowNumberUInt16InputDialogAsync(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemWeeksOffsetAsync(property.Id, value, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            calendar => calendar.Value = property.WeeksOffset,
            cancellationToken
        );
    }

    private static Task SetToDoMonthsOffsetAsync(
        IToDoMonthsOffsetProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowNumberUInt16InputDialogAsync(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemMonthsOffsetAsync(property.Id, value, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            calendar => calendar.Value = property.MonthsOffset,
            cancellationToken
        );
    }

    private static Task SetToDoDaysOffsetAsync(IToDoDaysOffsetProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowNumberUInt16InputDialogAsync(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemDaysOffsetAsync(property.Id, value, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            calendar => calendar.Value = property.DaysOffset,
            cancellationToken
        );
    }

    private static Task SetToDoDueDateAsync(IToDoDueDateProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowDateConfirmDialogAsync(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemDueDateAsync(property.Id, value.ToDateOnly(), cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            calendar => calendar.SelectedDate = property.DueDate.ToDateTime(),
            cancellationToken
        );
    }

    private static Task SetToDoChildrenTypeAsync(
        IToDoChildrenTypeProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowItemSelectorDialogAsync<ToDoItemChildrenType>(
            async item =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemChildrenTypeAsync(property.Id, item, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            viewModel =>
            {
                viewModel.Items.AddRange(Enum.GetValues<ToDoItemChildrenType>().OfType<object>());
                viewModel.SelectedItem = property.ChildrenType;
            },
            cancellationToken
        );
    }

    private static async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await objectStorage.DeleteAsync(FileIds.LoginFileId).ConfigureAwait(false);
        await navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken).ConfigureAwait(false);
        await cancellationToken.InvokeUIAsync(() => mainSplitViewModel.IsPaneOpen = false);
    }

    private static async Task LoginAsync(ILoginProperties properties, CancellationToken cancellationToken)
    {
        var user = mapper.Map<User>(properties);
        await tokenService.LoginAsync(user, cancellationToken).ConfigureAwait(false);
        properties.Account.Login = user.Login;
        await RememberMeAsync(properties, cancellationToken);

        await navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task RememberMeAsync(ILoginProperties properties, CancellationToken cancellationToken)
    {
        if (!properties.IsRememberMe)
        {
            return;
        }

        var token = await tokenService.GetTokenAsync(cancellationToken).ConfigureAwait(false);

        var item = new LoginStorageItem
        {
            Token = token,
        };

        await objectStorage.SaveObjectAsync(FileIds.LoginFileId, item).ConfigureAwait(false);
    }

    private static async Task NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        await navigator.NavigateToAsync(type, cancellationToken);
        await cancellationToken.InvokeUIAsync(() => mainSplitViewModel.IsPaneOpen = false);
    }

    private static Task BackAsync(CancellationToken cancellationToken)
    {
        return navigator.NavigateBackAsync(cancellationToken);
    }

    private static async Task CreateUserAsync(ICreateUserProperties properties, CancellationToken cancellationToken)
    {
        if (properties.Password != properties.RepeatPassword)
        {
            throw new("Password not equal RepeatPassword.");
        }

        var options = mapper.Map<CreateUserOptions>(properties);
        cancellationToken.ThrowIfCancellationRequested();
        await authenticationService.CreateUserAsync(options, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        await navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken).ConfigureAwait(false);
    }

    private static async Task SetDueDateTimeAsync(IDueDateTimeProperty property, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowDateTimeConfirmDialogAsync(
                async value =>
                {
                    await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await cancellationToken.InvokeUIBackgroundAsync(() => property.DueDateTime = value);
                },
                calendar =>
                {
                    calendar.SelectedDate = DateTimeOffset.Now.ToCurrentDay().DateTime;
                    calendar.SelectedTime = TimeSpan.Zero;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private static Task SetToDoShortItemAsync(IToDoShortItemProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async itemNotify =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                await cancellationToken.InvokeUIBackgroundAsync(
                    () => property.ShortItem = new()
                    {
                        Id = itemNotify.Id,
                        Name = itemNotify.Name
                    }
                );
            },
            view =>
            {
                if (property.ShortItem is null)
                {
                    return;
                }

                view.DefaultSelectedItemId = property.ShortItem.Id;
            },
            cancellationToken
        );
    }

    private static async Task RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await toDoService.RemoveFavoriteToDoItemAsync(id, cancellationToken).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken);
    }

    private static async Task AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        await toDoService.AddFavoriteToDoItemAsync(id, cancellationToken).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken);
    }

    private static async Task OpenLinkAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        var link = item.Link.ThrowIfNull().ToUri();
        cancellationToken.ThrowIfCancellationRequested();
        await openerLink.OpenLinkAsync(link, cancellationToken).ConfigureAwait(false);
    }

    private static async Task ChangeOrderIndexAsync(ToDoItemNotify item, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowConfirmContentDialogAsync<ChangeToDoItemOrderIndexViewModel>(
                async viewModel =>
                {
                    await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    var targetId = viewModel.SelectedItem.ThrowIfNull().Id;
                    var options = new UpdateOrderIndexToDoItemOptions(viewModel.Id, targetId, viewModel.IsAfter);
                    cancellationToken.ThrowIfCancellationRequested();
                    await toDoService.UpdateToDoItemOrderIndexAsync(options, cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    await RefreshCurrentViewAsync(cancellationToken);
                },
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                viewModel => viewModel.Id = item.Id,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private static async Task ChangeToActiveDoItemAsync(CancellationToken cancellationToken)
    {
        var item = await toDoService.GetCurrentActiveToDoItemAsync(cancellationToken).ConfigureAwait(false);

        if (item is null)
        {
            await navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await navigator.NavigateToAsync<ToDoItemViewModel>(view => view.Id = item.Value.Id, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static async Task DeleteSubToDoItemAsync(IDeletable deletable, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                async view =>
                {
                    await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                    await toDoService.DeleteToDoItemAsync(deletable.Id, cancellationToken)
                        .ConfigureAwait(false);

                    await RefreshCurrentViewAsync(cancellationToken);
                },
                _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
                view => view.Header = deletable.Header,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private static async Task SelectAllAsync(
        AvaloniaList<Selected<ToDoItemNotify>> items,
        CancellationToken cancellationToken
    )
    {
        await cancellationToken.InvokeUIAsync(
            () =>
            {
                if (items.All(x => x.IsSelect))
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = false;
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = true;
                    }
                }
            }
        );
    }

    private static Task CompleteToDoItemAsync(ICanComplete canComplete, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetCompleteStatus(canComplete.IsCan);

                viewModel.Complete = async status =>
                {
                    await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                    switch (status)
                    {
                        case CompleteStatus.Complete:
                            await toDoService.UpdateToDoItemCompleteStatusAsync(
                                    canComplete.Id,
                                    true,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Incomplete:
                            await toDoService.UpdateToDoItemCompleteStatusAsync(
                                    canComplete.Id,
                                    false,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Skip:
                            await toDoService.SkipToDoItemAsync(canComplete.Id, cancellationToken)
                                .ConfigureAwait(false);

                            break;
                        case CompleteStatus.Fail:
                            await toDoService.FailToDoItemAsync(canComplete.Id, cancellationToken)
                                .ConfigureAwait(false);

                            break;
                        default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
                    }

                    await RefreshCurrentViewAsync(cancellationToken);
                };
            },
            cancellationToken
        );
    }

    private static async Task RefreshCurrentViewAsync(CancellationToken cancellationToken)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return;
        }

        await refresh.RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Task NavigateToToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = id, cancellationToken);
    }

    private static async Task SwitchPaneAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await cancellationToken.InvokeUIAsync(() => mainSplitViewModel.IsPaneOpen = !mainSplitViewModel.IsPaneOpen);
    }

    public static CommandParameters CreateCommand(
        Func<CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask(work.RunAsync);
        SetupCommand(command);

        return new CommandParameters(new CommandItem(icon, command, name, null), work);
    }

    public static CommandParameters<TParam> CreateCommand<TParam>(
        Func<TParam, CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var work = TaskWork.Create(func);
        var command = ReactiveCommand.CreateFromTask<TParam>(work.RunAsync);
        SetupCommand(command);

        return new CommandParameters<TParam>(new CommandItem(icon, command, name, null), work);
    }

    private static void SetupCommand<TParam, TResult>(ReactiveCommand<TParam, TResult> command)
    {
        command.ThrownExceptions.Subscribe(OnNextError);
    }

    private static async void OnNextError(Exception exception)
    {
        if (exception is TaskCanceledException)
        {
            return;
        }

        Log.Logger.Error(exception, "UI error");

        await dialogViewer.ShowInfoErrorDialogAsync<ExceptionViewModel>(
            async _ =>
            {
                await dialogViewer.CloseErrorDialogAsync(CancellationToken.None);
                await dialogViewer.CloseProgressDialogAsync(CancellationToken.None);
            },
            viewModel => viewModel.Exception = exception,
            CancellationToken.None
        );
    }
}