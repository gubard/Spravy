using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
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
    IToDoSettingsProperty,
    IToDoNameProperty
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
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);

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

    public ICommand InitializedCommand { get; }

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
                CommandStorage.NavigateToCurrentToDoItemCommand,
                "Current to do item",
                null
            );
            pageHeaderViewModel.RightCommand = new CommandItem(
                MaterialIconKind.Pencil,
                CommandStorage.SetToDoItemNameCommand,
                "Edit name",
                this
            );
        }
    }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathViewModel PathViewModel { get; set; }

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

    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return RefreshAsync(cancellationToken);
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
                    CommandStorage.MultiCompleteToDoItemsCommand,
                    string.Empty,
                    ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                )
            );
            PageHeaderViewModel.Commands.Add(
                new CommandItem(
                    MaterialIconKind.Switch,
                    CommandStorage.MultiSetTypeToDoItemsCommand,
                    string.Empty,
                    ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
                )
            );
            PageHeaderViewModel.Commands.Add(
                new CommandItem(
                    MaterialIconKind.SwapHorizontal,
                    CommandStorage.MultiSetRootToDoItemsCommand,
                    string.Empty,
                    ToDoSubItemsViewModel.List.MultiToDoItems.GroupByNone.Items.Items
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
                new(
                    MaterialIconKind.Plus,
                    CommandStorage.AddToDoItemChildCommand,
                    "Add sub task",
                    PathViewModel.Items[^1].As<ToDoItemParentNotify>().ThrowIfNull().Id
                )
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
                new(
                    MaterialIconKind.SwapHorizontal,
                    CommandStorage.SetToDoParentItemCommand,
                    "Change task parent",
                    this
                )
            );
            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.FamilyTree, CommandStorage.MoveToDoItemToRootCommand, "Move to root task", this)
            );
            PageHeaderViewModel.Commands.Add(
                new(MaterialIconKind.CodeString, CommandStorage.ToDoItemToStringCommand, "Copy task to clipboard", this)
            );
            PageHeaderViewModel.Commands.Add(
                new(
                    MaterialIconKind.ContentCopy,
                    CommandStorage.ToDoItemRandomizeChildrenOrderIndexCommand,
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