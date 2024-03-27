using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Controls;
using Grpc.Core;
using Material.Icons;
using Ninject;
using Serilog;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Client.Exceptions;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.PasswordGenerator.ViewModels;
using Spravy.Ui.Features.ToDo.ViewModels;
using Spravy.Ui.Helpers;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Ui.Services;

public static class CommandStorage
{
    static CommandStorage()
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        clipboard = kernel.Get<IClipboardService>();
        passwordService = kernel.Get<IPasswordService>();
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
        SwitchCompleteToDoItemItem = CreateCommand<ICanCompleteProperty>(
            SwitchCompleteToDoItemAsync,
            MaterialIconKind.Check,
            "Complete"
        );
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
        OpenLinkItem = CreateCommand<ILink>(
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
            "Set to-do item"
        );
        SetDueDateTimeItem = CreateCommand<IDueDateTimeProperty>(
            SetDueDateTimeAsync,
            MaterialIconKind.Pencil,
            "Set due date time"
        );
        BackItem = CreateCommand(BackAsync, MaterialIconKind.ArrowLeft, "Back");
        NavigateToItem = CreateCommand<Type>(NavigateToAsync, MaterialIconKind.ArrowLeft, "Navigate to");
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
            "Add root to-do item"
        );
        ToDoItemSearchItem = CreateCommand<IToDoItemSearchProperties>(
            ToDoItemSearchAsync,
            MaterialIconKind.Search,
            "Search to-do item"
        );
        SetToDoTypeItem = CreateCommand<IToDoTypeProperty>(
            SetToDoTypeAsync,
            MaterialIconKind.Pencil,
            "Set to-do item type"
        );
        SetToDoLinkItem = CreateCommand<IToDoLinkProperty>(
            SetToDoLinkAsync,
            MaterialIconKind.Pencil,
            "Set to-do item link"
        );
        SetToDoDescriptionItem = CreateCommand<IToDoDescriptionProperty>(
            SetToDoDescriptionAsync,
            MaterialIconKind.Pencil,
            "Set to-do item description"
        );
        ShowToDoSettingItem = CreateCommand<IToDoSettingsProperty>(
            ShowToDoSettingAsync,
            MaterialIconKind.Settings,
            "Show to-do setting"
        );
        AddToDoItemChildItem = CreateCommand<IIdProperty>(
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
            "Set to-do item parent"
        );
        MoveToDoItemToRootItem = CreateCommand<IIdProperty>(
            MoveToDoItemToRootAsync,
            MaterialIconKind.FamilyTree,
            "Move to-do item to root"
        );
        ToDoItemToStringItem = CreateCommand<IIdProperty>(
            ToDoItemToStringAsync,
            MaterialIconKind.ContentCopy,
            "Copy to-do item"
        );
        ToDoItemRandomizeChildrenOrderIndexItem = CreateCommand<IIdProperty>(
            ToDoItemRandomizeChildrenOrderIndexAsync,
            MaterialIconKind.Dice6Outline,
            "Randomize children order"
        );
        NavigateToCurrentToDoItemItem = CreateCommand(
            NavigateToCurrentToDoItemAsync,
            MaterialIconKind.ArrowRight,
            "Open current to-do item"
        );
        SetToDoItemNameItem = CreateCommand<IToDoNameProperty>(
            SetToDoItemNameAsync,
            MaterialIconKind.Pencil,
            "Open current to-do item"
        );
        MultiCompleteToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSwitchCompleteToDoItemsAsync,
            MaterialIconKind.CheckAll,
            "Complete all to-do items"
        );
        MultiSetParentToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSetParentToDoItemsAsync,
            MaterialIconKind.SwapHorizontal,
            "Set parent for all to-do items"
        );
        MultiMoveToDoItemsToRootItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiMoveToDoItemsToRootAsync,
            MaterialIconKind.FamilyTree,
            "Move items to root"
        );
        MultiSetTypeToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiSetTypeToDoItemsAsync,
            MaterialIconKind.Switch,
            "Set type all to-do items"
        );
        VerificationEmailItem = CreateCommand<IVerificationEmail>(
            VerificationEmailAsync,
            MaterialIconKind.EmailVerified,
            "Verification email"
        );
        SendNewVerificationCodeItem = CreateCommand<IVerificationEmail>(
            SendNewVerificationCodeAsync,
            MaterialIconKind.CodeString,
            "Verification email"
        );
        UpdateEmailNotVerifiedUserByItem = CreateCommand<IVerificationEmail>(
            UpdateEmailNotVerifiedUserAsync,
            MaterialIconKind.EmailCheck,
            "Change email"
        );
        SetRequiredCompleteInDueDateItem = CreateCommand<IIsRequiredCompleteInDueDateProperty>(
            SetRequiredCompleteInDueDateAsync,
            MaterialIconKind.EmailCheck,
            "Required complete by the due date"
        );
        ResetToDoItemItem = CreateCommand<IIdProperty>(
            ResetToDoItemAsync,
            MaterialIconKind.EncryptionReset,
            "Reset to-do item"
        );
        MultiDeleteToDoItemsItem = CreateCommand<AvaloniaList<Selected<ToDoItemNotify>>>(
            MultiDeleteToDoItemsAsync,
            MaterialIconKind.Delete,
            "Delete to-do items"
        );
        CloneToDoItemItem = CreateCommand<ToDoItemNotify>(
            CloneToDoItemAsync,
            MaterialIconKind.FileMove,
            "Clone to-do item"
        );
        AddPasswordItemItem = CreateCommand(
            AddPasswordItemAsync,
            MaterialIconKind.Plus,
            "Add password item"
        );
        ShowPasswordItemSettingItem = CreateCommand<IIdProperty>(
            ShowPasswordItemSettingAsync,
            MaterialIconKind.Settings,
            "Show password setting"
        );
        GeneratePasswordItem = CreateCommand<IIdProperty>(
            GeneratePasswordAsync,
            MaterialIconKind.Regeneration,
            "Generate password"
        );
        RemovePasswordItemItem = CreateCommand<IIdProperty>(
            RemovePasswordItemAsync,
            MaterialIconKind.Delete,
            "Generate password"
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
    private static readonly IClipboardService clipboard;
    private static readonly IPasswordService passwordService;

    public static ICommand RemovePasswordItemCommand => RemovePasswordItemItem.Command;
    public static CommandItem RemovePasswordItemItem { get; }

    public static ICommand GeneratePasswordCommand => GeneratePasswordItem.Command;
    public static CommandItem GeneratePasswordItem { get; }

    public static ICommand CloneToDoItemCommand => CloneToDoItemItem.Command;
    public static CommandItem CloneToDoItemItem { get; }

    public static ICommand ResetToDoItemCommand => ResetToDoItemItem.Command;
    public static CommandItem ResetToDoItemItem { get; }

    public static ICommand SetRequiredCompleteInDueDateCommand => SetRequiredCompleteInDueDateItem.Command;
    public static CommandItem SetRequiredCompleteInDueDateItem { get; }

    public static ICommand UpdateEmailNotVerifiedUserByCommand => UpdateEmailNotVerifiedUserByItem.Command;
    public static CommandItem UpdateEmailNotVerifiedUserByItem { get; }

    public static ICommand SendNewVerificationCodeCommand => SendNewVerificationCodeItem.Command;
    public static CommandItem SendNewVerificationCodeItem { get; }

    public static ICommand VerificationEmailCommand => VerificationEmailItem.Command;
    public static CommandItem VerificationEmailItem { get; }

    public static ICommand MultiSetTypeToDoItemsCommand => MultiSetTypeToDoItemsItem.Command;
    public static CommandItem MultiSetTypeToDoItemsItem { get; }

    public static ICommand MultiMoveToDoItemsToRootCommand => MultiMoveToDoItemsToRootItem.Command;
    public static CommandItem MultiMoveToDoItemsToRootItem { get; }

    public static ICommand MultiSetParentToDoItemsCommand => MultiSetParentToDoItemsItem.Command;
    public static CommandItem MultiSetParentToDoItemsItem { get; }

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

    public static ICommand SwitchCompleteToDoItemCommand => SwitchCompleteToDoItemItem.Command;
    public static CommandItem SwitchCompleteToDoItemItem { get; }

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

    public static ICommand BackCommand => BackItem.Command;
    public static CommandItem BackItem { get; }

    public static ICommand NavigateToCommand => NavigateToItem.Command;
    public static CommandItem NavigateToItem { get; }

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

    public static ICommand MultiDeleteToDoItemsCommand => MultiDeleteToDoItemsItem.Command;
    public static CommandItem MultiDeleteToDoItemsItem { get; }

    public static ICommand AddPasswordItemCommand => AddPasswordItemItem.Command;
    public static CommandItem AddPasswordItemItem { get; }

    public static ICommand ShowPasswordItemSettingCommand => ShowPasswordItemSettingItem.Command;
    public static CommandItem ShowPasswordItemSettingItem { get; }

    public static CommandItem SelectAll { get; }

    private static Task RemovePasswordItemAsync(IIdProperty idProperty, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync<TextViewModel>(
            async _ =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                await passwordService.RemovePasswordItemAsync(idProperty.Id, cancellationToken)
                    .ConfigureAwait(false);

                await RefreshCurrentViewAsync(cancellationToken);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            view => view.Text = "Are you sure?",
            cancellationToken
        );
    }

    private static async Task GeneratePasswordAsync(IIdProperty idProperty, CancellationToken cancellationToken)
    {
        var password = await passwordService.GeneratePasswordAsync(idProperty.Id, cancellationToken);
        await clipboard.SetTextAsync(password);
    }

    private static Task ShowPasswordItemSettingAsync(IIdProperty idProperty, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync<PasswordItemSettingsViewModel>(
            async vm =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            vm => vm.Id = idProperty.Id,
            cancellationToken
        );
    }

    private static Task AddPasswordItemAsync(CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync(
            async vm =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                await passwordService.AddPasswordItemAsync(mapper.Map<AddPasswordOptions>(vm), cancellationToken)
                    .ConfigureAwait(false);

                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            ActionHelper<AddPasswordItemViewModel>.Empty,
            cancellationToken
        );
    }

    private static async Task MultiDeleteToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> items,
        CancellationToken cancellationToken
    )
    {
        var tasks = items.Where(x => x.IsSelect)
            .Select(x => x.Value.Id)
            .Select(x => toDoService.DeleteToDoItemAsync(x, cancellationToken))
            .ToArray();

        await Task.WhenAll(tasks).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task ResetToDoItemAsync(
        IIdProperty property,
        CancellationToken cancellationToken
    )
    {
        await toDoService.ResetToDoItemAsync(property.Id, cancellationToken).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task SetRequiredCompleteInDueDateAsync(
        IIsRequiredCompleteInDueDateProperty property,
        CancellationToken cancellationToken
    )
    {
        await toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                property.Id,
                property.IsRequiredCompleteInDueDate,
                cancellationToken
            )
            .ConfigureAwait(false);

        await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Task UpdateEmailNotVerifiedUserAsync(
        IVerificationEmail verificationEmail,
        CancellationToken cancellationToken
    )
    {
        return dialogViewer.ShowSingleStringConfirmDialogAsync(
            async newEmail =>
            {
                switch (verificationEmail.IdentifierType)
                {
                    case UserIdentifierType.Email:
                        await authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(
                            verificationEmail.Identifier,
                            newEmail,
                            cancellationToken
                        );
                        break;
                    case UserIdentifierType.Login:
                        await authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(
                            verificationEmail.Identifier,
                            newEmail,
                            cancellationToken
                        );
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                await dialogViewer.CloseInputDialogAsync(cancellationToken);
            },
            ActionHelper<TextViewModel>.Empty,
            cancellationToken
        );
    }

    private static Task SendNewVerificationCodeAsync(
        IVerificationEmail verificationEmail,
        CancellationToken cancellationToken
    )
    {
        switch (verificationEmail.IdentifierType)
        {
            case UserIdentifierType.Email:
                return authenticationService.UpdateVerificationCodeByEmailAsync(
                    verificationEmail.Identifier,
                    cancellationToken
                );

            case UserIdentifierType.Login:
                return authenticationService.UpdateVerificationCodeByLoginAsync(
                    verificationEmail.Identifier,
                    cancellationToken
                );
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task VerificationEmailAsync(
        IVerificationEmail verificationEmail,
        CancellationToken cancellationToken
    )
    {
        switch (verificationEmail.IdentifierType)
        {
            case UserIdentifierType.Email:
                await authenticationService.VerifiedEmailByEmailAsync(
                    verificationEmail.Identifier,
                    verificationEmail.VerificationCode.ToUpperInvariant(),
                    cancellationToken
                );

                await navigator.NavigateToAsync<LoginViewModel>(cancellationToken);

                return;
            case UserIdentifierType.Login:
                await authenticationService.VerifiedEmailByLoginAsync(
                    verificationEmail.Identifier,
                    verificationEmail.VerificationCode.ToUpperInvariant(),
                    cancellationToken
                );

                await navigator.NavigateToAsync<LoginViewModel>(cancellationToken);

                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task MultiMoveToDoItemsToRootAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        await Task.WhenAll(
                itemsNotify.Where(x => x.IsSelect)
                    .Select(x => toDoService.ToDoItemToRootAsync(x.Value.Id, cancellationToken))
            )
            .ConfigureAwait(false);

        await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Task MultiSetTypeToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();

        return dialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(
            async type =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                await Task.WhenAll(
                        ids.Select(x => toDoService.UpdateToDoItemTypeAsync(x, type, cancellationToken))
                    )
                    .ConfigureAwait(false);

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

    private static Task MultiSetParentToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var ids = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value.Id).ToArray();

        return dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async item =>
            {
                await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);

                await Task.WhenAll(
                        ids.Select(x => toDoService.UpdateToDoItemParentAsync(x, item.Id, cancellationToken))
                    )
                    .ConfigureAwait(false);

                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            viewModel => viewModel.IgnoreIds.AddRange(ids),
            cancellationToken
        );
    }

    private static async Task MultiSwitchCompleteToDoItemsAsync(
        AvaloniaList<Selected<ToDoItemNotify>> itemsNotify,
        CancellationToken cancellationToken
    )
    {
        var items = itemsNotify.Where(x => x.IsSelect).Select(x => x.Value).ToArray();
        await CompleteAsync(items, cancellationToken).ConfigureAwait(false);
        await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Task CompleteAsync(
        IEnumerable<ToDoItemNotify> items,
        CancellationToken cancellationToken
    )
    {
        var tasks = new List<Task>();

        foreach (var item in items)
        {
            switch (item.IsCan)
            {
                case ToDoItemIsCan.None:
                    break;
                case ToDoItemIsCan.CanComplete:
                    tasks.Add(toDoService.UpdateToDoItemCompleteStatusAsync(item.Id, true, cancellationToken));

                    break;
                case ToDoItemIsCan.CanIncomplete:
                    tasks.Add(toDoService.UpdateToDoItemCompleteStatusAsync(item.Id, false, cancellationToken));

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return Task.WhenAll(tasks);
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
            viewModel =>
            {
                viewModel.Text = "Are you sure?";
                viewModel.IsReadOnly = true;
            },
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

    private static Task AddToDoItemChildAsync(IIdProperty item, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
            async viewModel =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                var options = new AddToDoItemOptions(
                    viewModel.ParentId,
                    viewModel.ToDoItemContent.Name,
                    viewModel.ToDoItemContent.Type,
                    viewModel.DescriptionContent.Description,
                    viewModel.DescriptionContent.Type,
                    mapper.Map<Uri?>(viewModel.ToDoItemContent.Link)
                );

                await toDoService.AddToDoItemAsync(options, cancellationToken);
                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            vm => vm.ParentId = item.Id,
            cancellationToken
        );
    }

    private static Task ShowToDoSettingAsync(IToDoSettingsProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync<ToDoItemSettingsViewModel>(
            async vm =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                await Task.WhenAll(
                        toDoService.UpdateToDoItemNameAsync(property.Id, vm.ToDoItemContent.Name, cancellationToken),
                        toDoService.UpdateToDoItemTypeAsync(property.Id, vm.ToDoItemContent.Type, cancellationToken),
                        toDoService.UpdateToDoItemLinkAsync(
                            property.Id,
                            mapper.Map<Uri?>(vm.ToDoItemContent.Link),
                            cancellationToken
                        ),
                        vm.Settings.ThrowIfNull().ApplySettingsAsync(cancellationToken)
                    )
                    .ConfigureAwait(false);

                await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            vm => vm.ToDoItemId = property.Id,
            cancellationToken
        );
    }

    private static Task SetToDoDescriptionAsync(IToDoDescriptionProperty property, CancellationToken cancellationToken)
    {
        return dialogViewer.ShowConfirmContentDialogAsync<EditDescriptionViewModel>(
            async viewModel =>
            {
                await dialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                await toDoService.UpdateToDoItemDescriptionAsync(
                        property.Id,
                        viewModel.Content.Description,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                await toDoService.UpdateToDoItemDescriptionTypeAsync(
                        property.Id,
                        viewModel.Content.Type,
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                await property.RefreshAsync(cancellationToken).ConfigureAwait(false);
            },
            _ => dialogViewer.CloseContentDialogAsync(cancellationToken),
            viewModel =>
            {
                viewModel.Content.Description = property.Description;
                viewModel.Content.Type = property.DescriptionType;
                viewModel.ToDoItemName = property.Name;
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
        await properties.ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), properties, false, cancellationToken)
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
                                foreach (var day in month.Days)
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
        if (await objectStorage.IsExistsAsync(StorageIds.LoginId).ConfigureAwait(false))
        {
            await objectStorage.DeleteAsync(StorageIds.LoginId).ConfigureAwait(false);
        }

        await navigator.NavigateToAsync(ActionHelper<LoginViewModel>.Empty, cancellationToken).ConfigureAwait(false);
        await cancellationToken.InvokeUIAsync(() => mainSplitViewModel.IsPaneOpen = false);
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

    private static async Task OpenLinkAsync(ILink item, CancellationToken cancellationToken)
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

    private static async Task CloneToDoItemAsync(IIdProperty id, CancellationToken cancellationToken)
    {
        await dialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                async itemNotify =>
                {
                    await dialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await toDoService.CloneToDoItemAsync(id.Id, itemNotify.Id, cancellationToken);
                    await RefreshCurrentViewAsync(cancellationToken);
                },
                view => view.DefaultSelectedItemId = id.Id,
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
                view => view.ToDoItemName = deletable.Name,
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

    private static async Task SwitchCompleteToDoItemAsync(
        ICanCompleteProperty property,
        CancellationToken cancellationToken
    )
    {
        switch (property.IsCan)
        {
            case ToDoItemIsCan.None:
                break;
            case ToDoItemIsCan.CanComplete:
                await toDoService.UpdateToDoItemCompleteStatusAsync(
                        property.Id,
                        true,
                        cancellationToken
                    )
                    .ConfigureAwait(false);
                break;
            case ToDoItemIsCan.CanIncomplete:
                await toDoService.UpdateToDoItemCompleteStatusAsync(
                        property.Id,
                        false,
                        cancellationToken
                    )
                    .ConfigureAwait(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        await RefreshCurrentViewAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task RefreshCurrentViewAsync(CancellationToken cancellationToken)
    {
        if (mainSplitViewModel.Content is not IRefresh refresh)
        {
            return;
        }

        try
        {
            await refresh.RefreshAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (GrpcException e) when (e.InnerException is OperationCanceledException)
        {
        }
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

        if (exception is RpcException rpc)
        {
            switch (rpc.StatusCode)
            {
                case StatusCode.Cancelled:
                    return;
            }
        }

        if (exception is GrpcException { InnerException: RpcException rpc2 })
        {
            switch (rpc2.StatusCode)
            {
                case StatusCode.Cancelled:
                    return;
            }
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