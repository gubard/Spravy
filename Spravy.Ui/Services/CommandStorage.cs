using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Input.Platform;
using Material.Icons;
using Ninject;
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

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    static CommandStorage()
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        clipboard = kernel.Get<IClipboard>();
        objectStorage = kernel.Get<IObjectStorage>();
        mapper = kernel.Get<IMapper>();
        tokenService = kernel.Get<ITokenService>();
        authenticationService = kernel.Get<IAuthenticationService>();
        navigator = kernel.Get<INavigator>();
        openerLink = kernel.Get<IOpenerLink>();
        dialogViewer = kernel.Get<IDialogViewer>();
        mainSplitViewModel = kernel.Get<MainSplitViewModel>();
        toDoService = kernel.Get<IToDoService>();
        SwitchPaneItem = CreateCommand(SwitchPaneAsync, MaterialIconKind.Menu, "Open pane");
        NavigateToToDoItemItem = CreateCommand<Guid>(NavigateToToDoItemAsync, MaterialIconKind.ListBox, "Open");
        CompleteToDoItemItem = CreateCommand<ICanComplete>(CompleteToDoItemAsync, MaterialIconKind.Check, "Complete");
        SelectAll = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            SelectAllAsync,
            MaterialIconKind.CheckAll,
            "Select all"
        );
        DeleteToDoItemItem = CreateCommand<IDeletable>(DeleteToDoItemAsync, MaterialIconKind.Delete, "Delete");
        ChangeToActiveDoItemItem = CreateCommand(
            ChangeToActiveDoItemAsync,
            MaterialIconKind.ArrowRight,
            "Go to active"
        );
        ChangeOrderIndexItem = CreateCommand<ToDoItemNotify>(
            ChangeOrderIndexAsync,
            MaterialIconKind.ReorderHorizontal,
            "Reorder"
        );
        OpenLinkItem = CreateCommand<ToDoItemNotify>(
            OpenLinkAsync,
            MaterialIconKind.Link,
            "Open link"
        );
        RemoveToDoItemFromFavoriteItem = CreateCommand<Guid>(
            RemoveFavoriteToDoItemAsync,
            MaterialIconKind.Star,
            "Remove from favorite"
        );
        AddToDoItemToFavoriteItem = CreateCommand<Guid>(
            AddFavoriteToDoItemAsync,
            MaterialIconKind.StarOutline,
            "Add to favorite"
        );
        SetToDoShortItemItem = CreateCommand<IToDoShortItemProperty>(
            SetToDoShortItemAsync,
            MaterialIconKind.Pencil,
            "Set to do item"
        );
        SetDueDateTimeItem = CreateCommand<IDueDateTimeProperty>(
            SetDueDateTimeAsync,
            MaterialIconKind.Pencil,
            "Set due date time"
        );
        CreateUserItem = CreateCommand<ICreateUserProperties>(
            CreateUserAsync,
            MaterialIconKind.AccountPlus,
            "Set due date time"
        );
        BackItem = CreateCommand(BackAsync, MaterialIconKind.ArrowLeft, "Back");
        NavigateToItem = CreateCommand<Type>(NavigateToAsync, MaterialIconKind.ArrowLeft, "Navigate to");
        LoginItem = CreateCommand<ILoginProperties>(LoginAsync, MaterialIconKind.Login, "Login");
        LogoutItem = CreateCommand(LogoutAsync, MaterialIconKind.Logout, "Logout");
        SetToDoChildrenTypeItem = CreateCommand<IToDoChildrenTypeProperty>(
            SetToDoChildrenTypeAsync,
            MaterialIconKind.Pencil,
            "Set children type"
        );
        SetToDoDueDateItem = CreateCommand<IToDoDueDateProperty>(
            SetToDoDueDateAsync,
            MaterialIconKind.Pencil,
            "Set due date"
        );
        SetToDoDaysOffsetItem = CreateCommand<IToDoDaysOffsetProperty>(
            SetToDoDaysOffsetAsync,
            MaterialIconKind.Pencil,
            "Set days offset"
        );
        SetToDoMonthsOffsetItem = CreateCommand<IToDoMonthsOffsetProperty>(
            SetToDoMonthsOffsetAsync,
            MaterialIconKind.Pencil,
            "Set months offset"
        );
        SetToDoWeeksOffsetItem = CreateCommand<IToDoWeeksOffsetProperty>(
            SetToDoWeeksOffsetAsync,
            MaterialIconKind.Pencil,
            "Set weeks offset"
        );
        SetToDoYearsOffsetItem = CreateCommand<IToDoYearsOffsetProperty>(
            SetToDoYearsOffsetAsync,
            MaterialIconKind.Pencil,
            "Set years offset"
        );
        SetToDoTypeOfPeriodicityItem = CreateCommand<IToDoTypeOfPeriodicityProperty>(
            SetToDoTypeOfPeriodicityAsync,
            MaterialIconKind.Pencil,
            "Set type of periodicity"
        );
        SetToDoPeriodicityItem = CreateCommand<IToDoTypeOfPeriodicityProperty>(
            SetToDoPeriodicityAsync,
            MaterialIconKind.Pencil,
            "Set periodicity"
        );
        AddRootToDoItemItem = CreateCommand(
            AddRootToDoItemAsync,
            MaterialIconKind.Plus,
            "Add root to do item"
        );
        ToDoItemSearchItem = CreateCommand<IToDoItemSearchProperties>(
            ToDoItemSearchAsync,
            MaterialIconKind.Search,
            "Search to do item"
        );
        SetToDoTypeItem = CreateCommand<IToDoTypeProperty>(
            SetToDoTypeAsync,
            MaterialIconKind.Pencil,
            "Set to do item type"
        );
        SetToDoLinkItem = CreateCommand<IToDoLinkProperty>(
            SetToDoLinkAsync,
            MaterialIconKind.Pencil,
            "Set to do item link"
        );
        SetToDoDescriptionItem = CreateCommand<IToDoDescriptionProperty>(
            SetToDoDescriptionAsync,
            MaterialIconKind.Pencil,
            "Set to do item description"
        );
        ShowToDoSettingItem = CreateCommand<IToDoSettingsProperty>(
            ShowToDoSettingAsync,
            MaterialIconKind.Settings,
            "Show to do setting"
        );
        AddToDoItemChildItem = CreateCommand<Guid>(
            AddToDoItemChildAsync,
            MaterialIconKind.Plus,
            "Add child task"
        );
        NavigateToLeafItem = CreateCommand<Guid>(
            NavigateToLeafAsync,
            MaterialIconKind.Leaf,
            "Navigate to leaf"
        );
        SetToDoParentItemItem = CreateCommand<ISetToDoParentItemParams>(
            SetToDoParentItemAsync,
            MaterialIconKind.SwapHorizontal,
            "Set to do item parent"
        );
        MoveToDoItemToRootItem = CreateCommand<IIdProperty>(
            MoveToDoItemToRootAsync,
            MaterialIconKind.FamilyTree,
            "Move to do item to root"
        );
        ToDoItemToStringItem = CreateCommand<IIdProperty>(
            ToDoItemToStringAsync,
            MaterialIconKind.ContentCopy,
            "Copy to do item"
        );
        ToDoItemRandomizeChildrenOrderIndexItem = CreateCommand<IIdProperty>(
            ToDoItemRandomizeChildrenOrderIndexAsync,
            MaterialIconKind.Dice6Outline,
            "Randomize children order"
        );
        NavigateToCurrentToDoItemItem = CreateCommand(
            NavigateToCurrentToDoItemAsync,
            MaterialIconKind.ArrowRight,
            "Open current to do item"
        );
        SetToDoItemNameItem = CreateCommand<IToDoNameProperty>(
            SetToDoItemNameAsync,
            MaterialIconKind.Pencil,
            "Open current to do item"
        );
        MultiCompleteToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiCompleteToDoItemsAsync,
            MaterialIconKind.CheckAll,
            "Complete all to do items"
        );
        MultiSetRootToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSetRootToDoItemsAsync,
            MaterialIconKind.SwapHorizontal,
            "Set parent for all to do items"
        );
        MultiSetTypeToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSetTypeToDoItemsAsync,
            MaterialIconKind.Switch,
            "Set type all to do items"
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
    private static readonly IClipboard clipboard;

    public static ICommand MultiSetTypeToDoItemsCommand => MultiSetTypeToDoItemsItem.Command;
    public static CommandItem MultiSetTypeToDoItemsItem { get; }

    public static ICommand MultiSetRootToDoItemsCommand => MultiSetRootToDoItemsItem.Command;
    public static CommandItem MultiSetRootToDoItemsItem { get; }

    public static ICommand MultiCompleteToDoItemsCommand => MultiCompleteToDoItemsItem.Command;
    public static CommandItem MultiCompleteToDoItemsItem { get; }

    public static ICommand SetToDoItemNameCommand => SetToDoItemNameItem.Command;
    public static CommandItem SetToDoItemNameItem { get; }

    public static ICommand NavigateToCurrentToDoItemCommand => NavigateToCurrentToDoItemItem.Command;
    public static CommandItem NavigateToCurrentToDoItemItem { get; }


    public static ICommand ToDoItemRandomizeChildrenOrderIndexCommand =>
        ToDoItemRandomizeChildrenOrderIndexItem.Command;

    public static CommandItem ToDoItemRandomizeChildrenOrderIndexItem { get; }

    public static ICommand ToDoItemToStringCommand => ToDoItemToStringItem.Command;
    public static CommandItem ToDoItemToStringItem { get; }

    public static ICommand MoveToDoItemToRootCommand => MoveToDoItemToRootItem.Command;
    public static CommandItem MoveToDoItemToRootItem { get; }

    public static ICommand NavigateToLeafCommand => NavigateToLeafItem.Command;
    public static CommandItem NavigateToLeafItem { get; }

    public static ICommand AddToDoItemChildCommand => AddToDoItemChildItem.Command;
    public static CommandItem AddToDoItemChildItem { get; }

    public static ICommand ShowToDoSettingCommand => ShowToDoSettingItem.Command;
    public static CommandItem ShowToDoSettingItem { get; }

    public static ICommand SetToDoDescriptionCommand => SetToDoDescriptionItem.Command;
    public static CommandItem SetToDoDescriptionItem { get; }

    public static ICommand SetToDoLinkCommand => SetToDoLinkItem.Command;
    public static CommandItem SetToDoLinkItem { get; }

    public static ICommand SetToDoTypeCommand => SetToDoTypeItem.Command;
    public static CommandItem SetToDoTypeItem { get; }

    public static ICommand ToDoItemSearchCommand => ToDoItemSearchItem.Command;
    public static CommandItem ToDoItemSearchItem { get; }

    public static ICommand SwitchPaneCommand => SwitchPaneItem.Command;
    public static CommandItem SwitchPaneItem { get; }

    public static ICommand CompleteToDoItemCommand => CompleteToDoItemItem.Command;
    public static CommandItem CompleteToDoItemItem { get; }

    public static ICommand ChangeToActiveDoItemCommand => ChangeToActiveDoItemItem.Command;
    public static CommandItem ChangeToActiveDoItemItem { get; }

    public static ICommand ChangeOrderIndexCommand => ChangeOrderIndexItem.Command;
    public static CommandItem ChangeOrderIndexItem { get; }

    public static ICommand DeleteToDoItemCommand => DeleteToDoItemItem.Command;
    public static CommandItem DeleteToDoItemItem { get; }

    public static ICommand NavigateToToDoItemCommand => NavigateToToDoItemItem.Command;
    public static CommandItem NavigateToToDoItemItem { get; }

    public static ICommand OpenLinkCommand => OpenLinkItem.Command;
    public static CommandItem OpenLinkItem { get; }

    public static ICommand RemoveToDoItemFromFavoriteCommand => RemoveToDoItemFromFavoriteItem.Command;
    public static CommandItem RemoveToDoItemFromFavoriteItem { get; }

    public static ICommand AddToDoItemToFavoriteCommand => AddToDoItemToFavoriteItem.Command;
    public static CommandItem AddToDoItemToFavoriteItem { get; }

    public static ICommand SetToDoShortItemCommand => SetToDoShortItemItem.Command;
    public static CommandItem SetToDoShortItemItem { get; }

    public static ICommand SetDueDateTimeCommand => SetDueDateTimeItem.Command;
    public static CommandItem SetDueDateTimeItem { get; }

    public static ICommand CreateUserCommand => CreateUserItem.Command;
    public static CommandItem CreateUserItem { get; }

    public static ICommand BackCommand => BackItem.Command;
    public static CommandItem BackItem { get; }

    public static ICommand NavigateToCommand => NavigateToItem.Command;
    public static CommandItem NavigateToItem { get; }

    public static ICommand LoginCommand => LoginItem.Command;
    public static CommandItem LoginItem { get; }

    public static ICommand LogoutCommand => LogoutItem.Command;
    public static CommandItem LogoutItem { get; }

    public static ICommand SetToDoChildrenTypeCommand => SetToDoChildrenTypeItem.Command;
    public static CommandItem SetToDoChildrenTypeItem { get; }

    public static ICommand SetToDoDueDateCommand => SetToDoDueDateItem.Command;
    public static CommandItem SetToDoDueDateItem { get; }

    public static ICommand SetToDoDaysOffsetCommand => SetToDoDaysOffsetItem.Command;
    public static CommandItem SetToDoDaysOffsetItem { get; }

    public static ICommand SetToDoMonthsOffsetCommand => SetToDoMonthsOffsetItem.Command;
    public static CommandItem SetToDoMonthsOffsetItem { get; }

    public static ICommand SetToDoWeeksOffsetCommand => SetToDoWeeksOffsetItem.Command;
    public static CommandItem SetToDoWeeksOffsetItem { get; }

    public static ICommand SetToDoYearsOffsetCommand => SetToDoYearsOffsetItem.Command;
    public static CommandItem SetToDoYearsOffsetItem { get; }

    public static ICommand SetToDoTypeOfPeriodicityCommand => SetToDoTypeOfPeriodicityItem.Command;
    public static CommandItem SetToDoTypeOfPeriodicityItem { get; }

    public static ICommand SetToDoPeriodicityCommand => SetToDoPeriodicityItem.Command;
    public static CommandItem SetToDoPeriodicityItem { get; }

    public static ICommand AddRootToDoItemCommand => AddRootToDoItemItem.Command;
    public static CommandItem AddRootToDoItemItem { get; }

    public static ICommand SetToDoParentItemCommand => SetToDoParentItemItem.Command;
    public static CommandItem SetToDoParentItemItem { get; }

    public static CommandItem SelectAll { get; }

    private static Task MultiSetTypeToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(
            async type =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                foreach (var item in itemsNotify)
                {
                    if (!item.IsSelect)
                    {
                        continue;
                    }

                    await toDoService.UpdateToDoItemTypeAsync(item.Value.Id, type, cancellationToken)
                        .ConfigureAwait(false);
                }

                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            viewModel =>
            {
                viewModel.Items.AddRange(Enum.GetValues<ToDoItemType>().OfType<object>());
                viewModel.SelectedItem = viewModel.Items.First();
            },
            cancellationToken
        );
    }

    private static Task MultiSetRootToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();

        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async item =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                foreach (var id in ids)
                {
                    await toDoService.UpdateToDoItemParentAsync(id, item.Id, cancellationToken)
                        .ConfigureAwait(false);
                }

                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            viewModel => viewModel.IgnoreIds.AddRange(ids),
            cancellationToken
        );
    }

    private static Task MultiCompleteToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var items = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value).ToArray();

        return dialogViewer.ShowInfoInputDialogAsync<CompleteToDoItemViewModel>(
            _ => dialogViewer.CloseInputDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.SetAllStatus();

                viewModel.Complete = async status =>
                {
                    await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await CompleteAsync(items, status, cancellationToken).ConfigureAwait(false);
                    await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
                };
            },
            cancellationToken
        );
    }

    private static async Task CompleteAsync(
        IEnumerable<ToDoItemNotify> items,
        CompleteStatus status,
        CancellationToken cancellationToken
    )
    {
        switch (status)
        {
            case CompleteStatus.Complete:
                foreach (var item in items.Where(x => x.IsCan.HasFlag(ToDoItemIsCan.CanComplete)))
                {
                    await toDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Skip:
                foreach (var item in items.Where(x => x.IsCan.HasFlag(ToDoItemIsCan.CanSkip)))
                {
                    await toDoService.SkipToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Fail:
                foreach (var item in items.Where(x => x.IsCan.HasFlag(ToDoItemIsCan.CanFail)))
                {
                    await toDoService.FailToDoItemAsync(item.Id, cancellationToken).ConfigureAwait(false);
                }

                break;
            case CompleteStatus.Incomplete:
                foreach (var item in items.Where(x => x.IsCan.HasFlag(ToDoItemIsCan.CanIncomplete)))
                {
                    await toDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken)
                        .ConfigureAwait(false);
                }

                break;
            default: throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    private static Task SetToDoItemNameAsync(IToDoNameProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowSingleStringConfirmDialogAsync(
            async str =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemNameAsync(property.Id, str, cancellationToken)
                    .ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            box =>
            {
                box.Text = property.Name;
                box.Label = "Name";
            },
            cancellationToken
        );
    }

    private static async Task NavigateToCurrentToDoItemAsync(CancellationToken cancellationToken)
    {
        var activeToDoItem = await toDoService.GetCurrentActiveToDoItemAsync(cancellationToken).ConfigureAwait(false);

        if (activeToDoItem.HasValue)
        {
            await navigator.NavigateToAsync<ToDoItemViewModel>(
                    viewModel => viewModel.Id = activeToDoItem.Value.Id,
                    cancellationToken
                )
                .ConfigureAwait(false);
        }
        else
        {
            await navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static Task ToDoItemRandomizeChildrenOrderIndexAsync(
        IIdProperty property,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowConfirmContentDialogAsync<TextViewModel>(
            async _ =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.RandomizeChildrenOrderIndexAsync(property.Id, cancellationToken)
                    .ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            async _ => await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
            viewModel => viewModel.Text = "Are you sure?",
            cancellationToken
        );
    }

    private static Task ToDoItemToStringAsync(IIdProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            async view =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                var options = new ToDoItemToStringOptions(statuses, property.Id);
                cancellationToken.ThrowIfCancellationRequested();
                var text = await toDoService.ToDoItemToStringAsync(options, cancellationToken)
                    .ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                await clipboard.SetTextAsync(text).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            ActionHelper<ToDoItemToStringSettingsViewModel>.Empty,
            cancellationToken
        );
    }

    private static async Task MoveToDoItemToRootAsync(IIdProperty property, CancellationToken cancellationToken)
    {
        await toDoService.ToDoItemToRootAsync(property.Id, cancellationToken).ConfigureAwait(false);
        await navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
    }

    private static Task SetToDoParentItemAsync(ISetToDoParentItemParams property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async item =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemParentAsync(property.Id, item.Id, cancellationToken)
                    .ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            viewModel =>
            {
                viewModel.IgnoreIds.Add(property.Id);
                viewModel.DefaultSelectedItemId = property.ParentId.GetValueOrDefault();
            },
            cancellationToken
        );
    }

    private static Task NavigateToLeafAsync(Guid id, CancellationToken cancellationToken)
    {
        return navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Id = id, cancellationToken);
    }

    private static Task AddToDoItemChildAsync(Guid parentId, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            async viewModel =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                var options = new AddToDoItemOptions(parentId, viewModel.Name, viewModel.Type);
                await toDoService.AddToDoItemAsync(options, cancellationToken);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            ActionHelper<AddToDoItemViewModel>.Empty,
            cancellationToken
        );
    }

    private static Task ShowToDoSettingAsync(IToDoSettingsProperty property, CancellationToken cancellationToken)
    {
        return property.Type switch
        {
            ToDoItemType.Value => dialogViewer.ShowInfoContentDialogAsync<ValueToDoItemSettingsViewModel>(
                viewModel => viewModel.Id = property.Id,
                cancellationToken
            ),
            ToDoItemType.Planned => dialogViewer.ShowInfoContentDialogAsync<PlannedToDoItemSettingsViewModel>(
                viewModel => viewModel.Id = property.Id,
                cancellationToken
            ),
            ToDoItemType.Periodicity => dialogViewer.ShowInfoContentDialogAsync<PeriodicityToDoItemSettingsViewModel>(
                viewModel => viewModel.Id = property.Id,
                cancellationToken
            ),
            ToDoItemType.PeriodicityOffset => dialogViewer
                .ShowInfoContentDialogAsync<PeriodicityOffsetToDoItemSettingsViewModel>(
                    viewModel => viewModel.Id = property.Id,
                    cancellationToken
                ),
            ToDoItemType.Circle => dialogViewer.ShowInfoContentDialogAsync<ValueToDoItemSettingsViewModel>(
                viewModel => viewModel.Id = property.Id,
                cancellationToken
            ),
            ToDoItemType.Step => dialogViewer.ShowInfoContentDialogAsync<ValueToDoItemSettingsViewModel>(
                viewModel => viewModel.Id = property.Id,
                cancellationToken
            ),
            ToDoItemType.Group => throw new ArgumentOutOfRangeException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static Task SetToDoDescriptionAsync(IToDoDescriptionProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowMultiStringConfirmDialogAsync(
            async str =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemDescriptionAsync(property.Id, str, cancellationToken)
                    .ConfigureAwait(false);
                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
            },
            box =>
            {
                box.Text = property.Description;
                box.Label = "Description";
            },
            cancellationToken
        );
    }

    private static Task SetToDoLinkAsync(IToDoLinkProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowSingleStringConfirmDialogAsync(
            async value =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemLinkAsync(
                        property.Id,
                        value.IsNullOrWhiteSpace() ? null : value.ToUri(),
                        cancellationToken
                    )
                    .ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            textBox =>
            {
                textBox.Text = property.Link;
                textBox.Label = "Link";
            },
            cancellationToken
        );
    }

    private static Task SetToDoTypeAsync(IToDoTypeProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(
            async item =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                await toDoService.UpdateToDoItemTypeAsync(property.Id, item, cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            viewModel =>
            {
                viewModel.Items.AddRange(Enum.GetValues<ToDoItemType>().OfType<object>());
                viewModel.SelectedItem = property.Type;
            },
            cancellationToken
        );
    }

    private static async Task ToDoItemSearchAsync(
        IToDoItemSearchProperties properties,
        CancellationToken cancellationToken
    )
    {
        var ids = await toDoService.SearchToDoItemIdsAsync(properties.SearchText, cancellationToken)
            .ConfigureAwait(false);
        await properties.ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), properties, cancellationToken)
            .ConfigureAwait(false);
    }

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
        await objectStorage.DeleteAsync(StorageIds.LoginId).ConfigureAwait(false);
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

        await objectStorage.SaveObjectAsync(StorageIds.LoginId, item).ConfigureAwait(false);
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

    private static async Task DeleteToDoItemAsync(IDeletable deletable, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowConfirmContentDialogAsync<DeleteToDoItemViewModel>(
                async _ =>
                {
                    await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                    await toDoService.DeleteToDoItemAsync(deletable.Id, cancellationToken)
                        .ConfigureAwait(false);

                    if (deletable.IsNavigateToParent)
                    {
                        if (deletable.ParentId is null)
                        {
                            await navigator.NavigateToAsync<RootToDoItemsViewModel>(cancellationToken)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            await navigator.NavigateToAsync<ToDoItemViewModel>(
                                    viewModel => viewModel.Id = deletable.ParentId.Value,
                                    cancellationToken
                                )
                                .ConfigureAwait(false);
                        }

                        return;
                    }

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

    public static CommandItem CreateCommand(
        Func<CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var result = CommandItem.Create(icon, name, func);
        result.ThrownExceptions.Subscribe(OnNextError);

        return result;
    }

    public static CommandItem CreateCommand<TParam>(
        Func<TParam, CancellationToken, Task> func,
        MaterialIconKind icon,
        string name
    )
    {
        var result = CommandItem.Create(icon, name, func);
        result.ThrownExceptions.Subscribe(OnNextError);

        return result;
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