using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
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

public class ToDoItemViewModel : NavigatableViewModelBase,
    IToDoItemOrderChanger,
    ICanComplete,
    IToDoLinkProperty,
    IToDoDescriptionProperty,
    IToDoSettingsProperty
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
    private readonly ToDoSubItemsViewModel toDoSubItemsViewModel;
    private readonly PageHeaderViewModel pageHeaderViewModel;

    public ToDoItemViewModel() : base(true)
    {
        refreshWork = new(RefreshCoreAsync);
        refreshToDoItemWork = new(RefreshToDoItemCore);
        ChangeRootItemCommand = CreateCommandFromTask(TaskWork.Create(ChangeRootItemAsync).RunAsync);
        ToDoItemToRootCommand = CreateCommandFromTask(TaskWork.Create(ToDoItemToRootAsync).RunAsync);
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
        ToDoItemToStringCommand = CreateCommandFromTask(TaskWork.Create(ToDoItemToStringAsync).RunAsync);
        AddTimerCommand = CreateCommandFromTask(TaskWork.Create(AddTimerAsync).RunAsync);
        RandomizeChildrenOrderIndexCommand =
            CreateCommandFromTask(TaskWork.Create(RandomizeChildrenOrderIndexAsync).RunAsync);
        ToCurrentToDoItemCommand = CreateCommandFromTask(TaskWork.Create(ToCurrentToDoItemAsync).RunAsync);
        ChangeNameCommand = CreateCommandFromTask(TaskWork.Create(ChangeNameAsync).RunAsync);

        this.WhenAnyValue(x => x.Name)
            .Subscribe(
                x =>
                {
                    if (PageHeaderViewModel is null)
                    {
                        return;
                    }

                    PageHeaderViewModel.Header = x;
                }
            );
    }

    public ICommand ChangeRootItemCommand { get; }
    public ICommand ToDoItemToRootCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand ToDoItemToStringCommand { get; }
    public ICommand AddTimerCommand { get; }
    public ICommand RandomizeChildrenOrderIndexCommand { get; }
    public ICommand ToCurrentToDoItemCommand { get; }
    public ICommand ChangeNameCommand { get; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel
    {
        get => toDoSubItemsViewModel;
        [MemberNotNull(nameof(toDoSubItemsViewModel))]
        init
        {
            toDoSubItemsViewModel = value;
            toDoSubItemsViewModel.List.WhenAnyValue(x => x.IsMulti).Skip(1).Subscribe(_ => UpdateCommands());
        }
    }

    [Inject]
    public required PageHeaderViewModel PageHeaderViewModel
    {
        get => pageHeaderViewModel;
        [MemberNotNull(nameof(pageHeaderViewModel))]
        init
        {
            pageHeaderViewModel = value;
            pageHeaderViewModel.Header = Name;
            pageHeaderViewModel.LeftCommand = new CommandItem(
                MaterialIconKind.ArrowRight,
                ToCurrentToDoItemCommand,
                "Current to do item",
                null
            );
            pageHeaderViewModel.RightCommand = new CommandItem(
                MaterialIconKind.Pencil,
                ChangeNameCommand,
                "Edit name",
                null
            );
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

    private Task RandomizeChildrenOrderIndexAsync(CancellationToken cancellationToken)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<TextViewModel>(
            async _ =>
            {
                await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false);
                await ToDoService.RandomizeChildrenOrderIndexAsync(Id, cancellationToken).ConfigureAwait(false);
                await RefreshAsync(cancellationToken).ConfigureAwait(false);
            },
            async _ => await DialogViewer.CloseContentDialogAsync(cancellationToken).ConfigureAwait(false),
            viewModel => viewModel.Text = "Are you sure?",
            cancellationToken
        );
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return refreshWork.RunAsync();
    }

    private Task RefreshCoreAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(
                RefreshToDoItemAsync(),
                RefreshPathAsync(cancellationToken),
                RefreshToDoItemChildrenAsync(cancellationToken),
                UpdateCommandsAsync()
            )
            .WaitAsync(cancellationToken);
    }

    private Task RefreshToDoItemAsync()
    {
        return refreshToDoItemWork.RunAsync();
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

    private Task AddTimerAsync(CancellationToken cancellationToken)
    {
        return DialogViewer.ShowConfirmContentDialogAsync<AddTimerViewModel>(
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
        );
    }

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
    }

    private Task ToDoItemToStringAsync(CancellationToken cancellationToken)
    {
        return DialogViewer.ShowConfirmContentDialogAsync(
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
        );
    }

    private Task ChangeRootItemAsync(CancellationToken cancellationToken)
    {
        return DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
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
        );
    }

    private async Task ToDoItemToRootAsync(CancellationToken cancellationToken)
    {
        await ToDoService.ToDoItemToRootAsync(Id, cancellationToken).ConfigureAwait(false);
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
        PageHeaderViewModel.Commands.Clear();

        if (ToDoSubItemsViewModel.List.IsMulti)
        {
            PageHeaderViewModel.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    ToDoSubItemsViewModel.MultiCompleteCommand,
                    string.Empty,
                    null
                )
            );
            PageHeaderViewModel.Commands.Add(
                new CommandItem(
                    MaterialIconKind.Switch,
                    ToDoSubItemsViewModel.MultiChangeTypeCommand,
                    string.Empty,
                    null
                )
            );
            PageHeaderViewModel.Commands.Add(
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
                CommandStorage.AddToDoItemToFavoriteCommand,
                "Move to favorite",
                null
            );

            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.Plus, CommandStorage.AddToDoItemChildCommand, "Add sub task", null)
            );

            if (IsCan != ToDoItemIsCan.None)
            {
                PageHeaderViewModel.Commands.Add(
                    new(MaterialIconKind.Check, CommandStorage.CompleteToDoItemCommand, "Complete", this)
                );
            }

            if (Type != ToDoItemType.Group)
            {
                PageHeaderViewModel.Commands.Add(
                    new(MaterialIconKind.Settings, CommandStorage.ShowToDoSettingCommand, "Settings", this)
                );
            }

            if (IsFavorite)
            {
                toFavoriteCommand = new CommandItem(
                    MaterialIconKind.Star,
                    CommandStorage.RemoveToDoItemFromFavoriteCommand,
                    "Remove from favorite",
                    null
                );
            }

            PageHeaderViewModel.Commands.Add(toFavoriteCommand);
            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.Leaf, CommandStorage.NavigateToLeafCommand, "Show all children", Id)
            );
            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.SwapHorizontal, ChangeRootItemCommand, "Change task parent", null)
            );
            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.FamilyTree, ToDoItemToRootCommand, "Move to root task", null)
            );
            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.CodeString, ToDoItemToStringCommand, "Copy task to clipboard", null)
            );
            PageHeaderViewModel.Commands.Add(new(MaterialIconKind.Timer, AddTimerCommand, "Add timer", null));

            PageHeaderViewModel.Commands.Add(
                new(
                    MaterialIconKind.Dice6Outline,
                    RandomizeChildrenOrderIndexCommand,
                    "Randomize sub tasks",
                    null
                )
            );
        }
    }

    public override void Stop()
    {
        refreshToDoItemWork.Cancel();
        refreshWork.Cancel();
    }
}