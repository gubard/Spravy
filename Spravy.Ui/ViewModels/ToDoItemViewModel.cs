using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Input.Platform;
using Google.Protobuf;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Models;
using Spravy.EventBus.Domain.Helpers;
using Spravy.EventBus.Protos;
using Spravy.Schedule.Domain.Interfaces;
using Spravy.Schedule.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class ToDoItemViewModel : NavigatableViewModelBase, IToDoItemOrderChanger, IPageHeaderDataType, ICanComplete
{
    private string name = string.Empty;
    private Guid id;
    private string description = string.Empty;
    private ToDoItemType type;
    private bool isFavorite;
    private string link = string.Empty;
    private ToDoItemIsCan isCan;
    private readonly TaskWork refreshToDoItemWork;
    private readonly TaskWork refreshWork;
    private ToDoItemStatus status;
    private object? header;
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;

    public ToDoItemViewModel() : base(true)
    {
        refreshWork = new(RefreshCoreAsync);
        refreshToDoItemWork = new(RefreshToDoItemCore);
        RemoveToDoItemFromFavoriteCommand =
            CreateCommandFromTask(TaskWork.Create(RemoveToDoItemFromFavoriteAsync).RunAsync);
        ChangeDescriptionCommand = CreateCommandFromTask(TaskWork.Create(ChangeDescriptionAsync).RunAsync);
        AddToDoItemCommand = CreateCommandFromTask(TaskWork.Create(AddToDoItemAsync).RunAsync);
        ChangeToDoItemByPathCommand = CreateCommandFromTask<ToDoItemParentNotify>(
            TaskWork.Create<ToDoItemParentNotify>(ChangeToDoItemByPathAsync).RunAsync
        );
        ToRootItemCommand = CreateCommandFromTask(TaskWork.Create(ToRootItemAsync).RunAsync);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
        ChildrenTypes = new(Enum.GetValues<ToDoItemChildrenType>());
        AddToDoItemToFavoriteCommand = CreateCommandFromTask(TaskWork.Create(AddToDoItemToCurrentAsync).RunAsync);
        ToLeafToDoItemsCommand = CreateCommandFromTask(TaskWork.Create(ToLeafToDoItemsAsync).RunAsync);
        ChangeRootItemCommand = CreateCommandFromTask(TaskWork.Create(ChangeRootItemAsync).RunAsync);
        ToDoItemToRootCommand = CreateCommandFromTask(TaskWork.Create(ToDoItemToRootAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ToDoItemToStringCommand = CreateCommandFromTask(TaskWork.Create(ToDoItemToStringAsync).RunAsync);
        AddTimerCommand = CreateCommandFromTask(TaskWork.Create(AddTimerAsync).RunAsync);
        ChangeLinkCommand = CreateCommandFromTask(TaskWork.Create(ChangeLinkAsync).RunAsync);
        ChangeTypeCommand = CreateCommandFromTask(TaskWork.Create(ChangeTypeAsync).RunAsync);
        SettingsToDoItemCommand = CreateCommandFromTask(TaskWork.Create(SettingsToDoItemAsync).RunAsync);
        RandomizeChildrenOrderIndexCommand =
            CreateCommandFromTask(TaskWork.Create(RandomizeChildrenOrderIndexAsync).RunAsync);
        ToCurrentToDoItemCommand = CreateCommandFromTask(TaskWork.Create(ToCurrentToDoItemAsync).RunAsync);
        ChangeNameCommand = CreateCommandFromTask(TaskWork.Create(ChangeNameAsync).RunAsync);
        this.WhenAnyValue(x => x.Name).Subscribe(x => Header = x);
        LeftCommand = new CommandItem(
            MaterialIconKind.ArrowRight,
            ToCurrentToDoItemCommand,
            "Current to do item",
            null
        );
        RightCommand = new CommandItem(
            MaterialIconKind.Pencil,
            ChangeNameCommand,
            "Edit name",
            null
        );
    }

    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; }
    public ICommand SettingsToDoItemCommand { get; }
    public ICommand ChangeDescriptionCommand { get; }
    public ICommand AddToDoItemToFavoriteCommand { get; }
    public ICommand RemoveToDoItemFromFavoriteCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand ChangeToDoItemByPathCommand { get; }
    public ICommand ToRootItemCommand { get; }
    public ICommand ToLeafToDoItemsCommand { get; }
    public ICommand ChangeRootItemCommand { get; }
    public ICommand ToDoItemToRootCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand ToDoItemToStringCommand { get; }
    public ICommand AddTimerCommand { get; }
    public ICommand ChangeLinkCommand { get; }
    public ICommand ChangeTypeCommand { get; }
    public ICommand RandomizeChildrenOrderIndexCommand { get; }
    public ICommand ToCurrentToDoItemCommand { get; }
    public ICommand ChangeNameCommand { get; }
    public CommandItem? LeftCommand { get; } = null;
    public CommandItem? RightCommand { get; } = null;
    public AvaloniaList<CommandItem> Commands { get; } = new();


    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;
            toDoSubItemsViewModel.List.WhenAnyValue(x => x.IsMulti).Subscribe(_ => UpdateCommands());
        }
    }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IScheduleService ScheduleService { get; init; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathViewModel PathViewModel { get; set; }

    [Inject]
    public required IClipboard Clipboard { get; set; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    public object? Header
    {
        get => header;
        set => this.RaiseAndSetIfChanged(ref header, value);
    }

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }

    public ToDoItemType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public string Link
    {
        get => link;
        set => this.RaiseAndSetIfChanged(ref link, value);
    }

    public ToDoItemIsCan IsCan
    {
        get => isCan;
        set => this.RaiseAndSetIfChanged(ref isCan, value);
    }

    public ToDoItemStatus Status
    {
        get => status;
        set => this.RaiseAndSetIfChanged(ref status, value);
    }

    private async Task ChangeNameAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowSingleStringConfirmDialogAsync(
                async str =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemNameAsync(Id, str, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                box =>
                {
                    box.Text = Name;
                    box.Label = "Name";
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ToCurrentToDoItemAsync(CancellationToken cancellationToken)
    {
        var activeToDoItem = await ToDoService.GetCurrentActiveToDoItemAsync(cancellationToken).ConfigureAwait(false);

        if (activeToDoItem.HasValue)
        {
            await Navigator.NavigateToAsync<ToDoItemViewModel>(
                    viewModel => viewModel.Id = activeToDoItem.Value.Id,
                    cancellationToken
                )
                .ConfigureAwait(false);
        }
        else
        {
            await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private async Task RandomizeChildrenOrderIndexAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        await DialogViewer.ShowConfirmContentDialogAsync<TextViewModel>(
                async _ =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.RandomizeChildrenOrderIndexAsync(Id, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                async _ => await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
                viewModel => viewModel.Text = "Are you sure?",
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task SettingsToDoItemAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        switch (Type)
        {
            case ToDoItemType.Value:
                await DialogViewer.ShowInfoContentDialogAsync<ValueToDoItemSettingsViewModel>(
                        viewModel =>
                        {
                            viewModel.Refresh = this;
                            viewModel.Id = Id;
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            case ToDoItemType.Planned:
                await DialogViewer.ShowInfoContentDialogAsync<PlannedToDoItemSettingsViewModel>(
                        viewModel =>
                        {
                            viewModel.Refresh = this;
                            viewModel.Id = Id;
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            case ToDoItemType.Periodicity:
                await DialogViewer.ShowInfoContentDialogAsync<PeriodicityToDoItemSettingsViewModel>(
                        viewModel =>
                        {
                            viewModel.Refresh = this;
                            viewModel.Id = Id;
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            case ToDoItemType.PeriodicityOffset:
                await DialogViewer.ShowInfoContentDialogAsync<PeriodicityOffsetToDoItemSettingsViewModel>(
                        viewModel =>
                        {
                            viewModel.Refresh = this;
                            viewModel.Id = Id;
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            case ToDoItemType.Circle:
                await DialogViewer.ShowInfoContentDialogAsync<ValueToDoItemSettingsViewModel>(
                        viewModel =>
                        {
                            viewModel.Refresh = this;
                            viewModel.Id = Id;
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);

                break;
            case ToDoItemType.Step:
                await DialogViewer.ShowInfoContentDialogAsync<ValueToDoItemSettingsViewModel>(
                        viewModel =>
                        {
                            viewModel.Refresh = this;
                            viewModel.Id = Id;
                        },
                        cancellationToken
                    )
                    .ConfigureAwait(false);
                break;
            case ToDoItemType.Group: throw new ArgumentOutOfRangeException();
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private async Task ChangeTypeAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowItemSelectorDialogAsync<ToDoItemType>(
                async item =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemTypeAsync(Id, item, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken);
                },
                viewModel =>
                {
                    viewModel.Items.AddRange(Enum.GetValues<ToDoItemType>().OfType<object>());
                    viewModel.SelectedItem = Type;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync();
    }

    private async Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        await Task.WhenAll(
                RefreshToDoItemAsync(),
                RefreshPathAsync(cancellationToken),
                RefreshToDoItemChildrenAsync(cancellationToken),
                UpdateCommandsAsync()
            )
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task RefreshToDoItemAsync()
    {
        await refreshToDoItemWork.RunAsync();
    }

    private async Task RefreshToDoItemCore(CancellationToken cancellationToken)
    {
        var item = await ToDoService.GetToDoItemAsync(Id, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                Link = item.Link?.AbsoluteUri ?? string.Empty;
                Description = item.Description;
                Name = item.Name;
                Type = item.Type;
                IsCan = item.IsCan;
                IsFavorite = item.IsFavorite;
                Status = item.Status;
            }
        );
    }


    private async Task RefreshPathAsync(CancellationToken cancellationToken)
    {
        var parents = await ToDoService.GetParentsAsync(Id, cancellationToken).ConfigureAwait(false);

        await this.InvokeUIBackgroundAsync(
            () =>
            {
                PathViewModel.Items.Clear();
                PathViewModel.Items.Add(new RootItem());
                PathViewModel.Items.AddRange(parents.Select(x => Mapper.Map<ToDoItemParentNotify>(x)));
            }
        );
    }

    private async Task RefreshToDoItemChildrenAsync(CancellationToken cancellationToken)
    {
        var ids = await ToDoService.GetChildrenToDoItemIdsAsync(Id, cancellationToken).ConfigureAwait(false);
        await ToDoSubItemsViewModel.UpdateItemsAsync(ids.ToArray(), this, cancellationToken).ConfigureAwait(false);
    }

    private async Task ChangeLinkAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowSingleStringConfirmDialogAsync(
                async value =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemLinkAsync(
                            Id,
                            value.IsNullOrWhiteSpace() ? null : value.ToUri(),
                            cancellationToken
                        )
                        .ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                textBox =>
                {
                    textBox.Text = Link;
                    textBox.Label = "Link";
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task AddTimerAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        await DialogViewer.ShowConfirmContentDialogAsync<AddTimerViewModel>(
                async viewModel =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);

                    var eventValue = new ChangeToDoItemIsFavoriteEvent
                    {
                        IsFavorite = IsFavorite,
                        ToDoItemId = Mapper.Map<ByteString>(viewModel.ShortItem.ThrowIfNull().Id),
                    };

                    await using var stream = new MemoryStream();
                    eventValue.WriteTo(stream);
                    stream.Position = 0;

                    var parameters = new AddTimerParameters(
                        viewModel.DueDateTime,
                        viewModel.EventId,
                        await stream.ToByteArrayAsync()
                    );

                    cancellationToken.ThrowIfCancellationRequested();
                    await ScheduleService.AddTimerAsync(parameters, cancellationToken).ConfigureAwait(false);
                },
                async _ => await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
                viewModel =>
                {
                    viewModel.EventId = EventIdHelper.ChangeFavoriteId;

                    viewModel.ShortItem = new()
                    {
                        Id = Id,
                        Name = Name,
                    };

                    viewModel.DueDateTime = DateTimeOffset.Now.ToCurrentDay();
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeDescriptionAsync(CancellationToken cancellationToken)
    {
        await DialogViewer.ShowMultiStringConfirmDialogAsync(
                async str =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemDescriptionAsync(Id, str, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                box =>
                {
                    box.Text = Description;
                    box.Label = "Description";
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task ToDoItemToStringAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        await DialogViewer.ShowConfirmContentDialogAsync(
                async view =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    var statuses = view.Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                    var options = new ToDoItemToStringOptions(statuses, Id);
                    cancellationToken.ThrowIfCancellationRequested();
                    var text = await ToDoService.ToDoItemToStringAsync(options, cancellationToken)
                        .ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    await Clipboard.SetTextAsync(text).ConfigureAwait(false);
                },
                _ => DialogViewer.CloseContentDialogAsync(cancellationToken),
                ActionHelper<ToDoItemToStringSettingsViewModel>.Empty,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeRootItemAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        await DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
                async item =>
                {
                    await DialogViewer.CloseInputDialogAsync(cancellationToken).ConfigureAwait(false);
                    await ToDoService.UpdateToDoItemParentAsync(Id, item.Id, cancellationToken).ConfigureAwait(false);
                    await RefreshAsync(cancellationToken).ConfigureAwait(false);
                },
                viewModel =>
                {
                    viewModel.IgnoreIds.Add(Id);
                    var parents = PathViewModel.Items.OfType<ToDoItemParentNotify>().ToArray();

                    if (parents.Length == 1)
                    {
                        return;
                    }

                    viewModel.DefaultSelectedItemId = parents[^2].Id;
                },
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ToLeafToDoItemsAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);
        await Navigator.NavigateToAsync<LeafToDoItemsViewModel>(vm => vm.Id = Id, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task RemoveToDoItemFromFavoriteAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);
        await ToDoService.RemoveFavoriteToDoItemAsync(Id, cancellationToken).ConfigureAwait(false);
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task AddToDoItemToCurrentAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);
        await ToDoService.AddFavoriteToDoItemAsync(Id, cancellationToken).ConfigureAwait(false);
        await RefreshAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task ToDoItemToRootAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);
        await ToDoService.ToDoItemToRootAsync(Id, cancellationToken).ConfigureAwait(false);
        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task AddToDoItemAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        await DialogViewer.ShowConfirmContentDialogAsync<AddToDoItemViewModel>(
                async viewModel =>
                {
                    await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                    var parentValue = viewModel.Parent.ThrowIfNull();
                    var options = new AddToDoItemOptions(parentValue.Id, viewModel.Name, viewModel.Type);
                    await ToDoService.AddToDoItemAsync(options, cancellationToken);

                    await Navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = parentValue.Id, cancellationToken)
                        .ConfigureAwait(false);
                },
                async _ => await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
                viewModel => viewModel.Parent = Mapper.Map<ToDoItemNotify>(this),
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    private async Task ChangeToDoItemByPathAsync(ToDoItemParentNotify item, CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);

        await Navigator.NavigateToAsync<ToDoItemViewModel>(vm => vm.Id = item.Id, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task ToRootItemAsync(CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(HideFlyout);
        await Navigator.NavigateToAsync(ActionHelper<RootToDoItemsViewModel>.Empty, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task UpdateCommandsAsync()
    {
        await refreshToDoItemWork.Current;
        await this.InvokeUIBackgroundAsync(UpdateCommands);
    }

    private void UpdateCommands()
    {
        Commands.Clear();

        if (ToDoSubItemsViewModel.List.IsMulti)
        {
            Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    ToDoSubItemsViewModel.MultiCompleteCommand,
                    string.Empty,
                    null
                )
            );
            Commands.Add(
                new CommandItem(
                    MaterialIconKind.Switch,
                    ToDoSubItemsViewModel.MultiChangeTypeCommand,
                    string.Empty,
                    null
                )
            );
            Commands.Add(
                new CommandItem(
                    MaterialIconKind.SwapHorizontal,
                    ToDoSubItemsViewModel.MultiChangeRootItemCommand,
                    string.Empty,
                    null
                )
            );
        }
        else
        {
            var toFavoriteCommand = new CommandItem(
                MaterialIconKind.StarOutline,
                AddToDoItemToFavoriteCommand,
                "Move to favorite",
                null
            );

            Commands.Add(new(MaterialIconKind.Plus, AddToDoItemCommand, "Add sub task", null));

            if (IsCan != ToDoItemIsCan.None)
            {
                Commands.Add(
                    new(MaterialIconKind.Check, CommandStorage.CompleteToDoItemCommand, "Complete", this)
                );
            }

            if (Type != ToDoItemType.Group)
            {
                Commands.Add(
                    new(MaterialIconKind.Settings, SettingsToDoItemCommand, "Settings", null)
                );
            }

            if (IsFavorite)
            {
                toFavoriteCommand = new CommandItem(
                    MaterialIconKind.Star,
                    RemoveToDoItemFromFavoriteCommand,
                    "Remove from favorite",
                    null
                );
            }

            Commands.Add(toFavoriteCommand);
            Commands.Add(
                new(MaterialIconKind.Leaf, ToLeafToDoItemsCommand, "Show all children", null)
            );
            Commands.Add(
                new(MaterialIconKind.SwapHorizontal, ChangeRootItemCommand, "Change task parent", null)
            );
            Commands.Add(
                new(MaterialIconKind.FamilyTree, ToDoItemToRootCommand, "Move to root task", null)
            );
            Commands.Add(
                new(MaterialIconKind.CodeString, ToDoItemToStringCommand, "Copy task to clipboard", null)
            );
            Commands.Add(new(MaterialIconKind.Timer, AddTimerCommand, "Add timer", null));

            Commands.Add(
                new(
                    MaterialIconKind.Dice6Outline,
                    RandomizeChildrenOrderIndexCommand,
                    "Randomize sub tasks",
                    null
                )
            );
        }
    }

    private void HideFlyout()
    {
    }

    public override void Stop()
    {
        refreshToDoItemWork.Cancel();
        refreshWork.Cancel();
    }

    private async Task SelectAllAsync(AvaloniaList<Selected<ToDoItemNotify>> items, CancellationToken arg)
    {
        await this.InvokeUIAsync(
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
}