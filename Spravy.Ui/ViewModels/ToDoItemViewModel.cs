using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Input.Platform;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.EventBus.Domain.Helpers;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Controls;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public abstract class ToDoItemViewModel : RoutableViewModelBase, IToDoItemOrderChanger
{
    private string name = string.Empty;
    private Guid id;
    private string description = string.Empty;

    protected readonly List<IDisposable> PropertySubscribes = new();
    private ToDoItemType type;
    private bool isFavorite;
    protected readonly List<ToDoItemCommand> Commands = new();

    public ToDoItemViewModel(string? urlPathSegment) : base(urlPathSegment)
    {
        RemoveToDoItemFromFavoriteCommand =
            CreateCommandFromTaskWithDialogProgressIndicator(RemoveToDoItemFromFavoriteAsync);

        var toFavoriteCommand = new ToDoItemCommand(MaterialIconKind.Star, RemoveToDoItemFromFavoriteCommand);
        ChangeDescriptionCommand = CreateCommandFromTask(ChangeDescriptionAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        ChangeToDoItemByPathCommand = CreateCommandFromTask<ToDoItemParentNotify>(ChangeToDoItemByPathAsync);
        ToRootItemCommand = CreateCommand(ToRootItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
        ChildrenTypes = new(Enum.GetValues<ToDoItemChildrenType>());
        AddToDoItemToFavoriteCommand = CreateCommandFromTaskWithDialogProgressIndicator(AddToDoItemToCurrentAsync);
        ToLeafToDoItemsCommand = CreateCommand(ToLeafToDoItems);
        ChangeRootItemCommand = CreateCommandFromTask(ChangeRootItemAsync);
        ToDoItemToRootCommand = CreateCommandFromTask(ToDoItemToRootAsync);
        InitializedCommand = CreateCommand(Initialized);
        ToDoItemToStringCommand = CreateCommandFromTask(ToDoItemToStringAsync);
        AddTimerCommand = CreateCommand(AddTimer);

        this.WhenAnyValue(x => x.IsFavorite)
            .Subscribe(
                x =>
                {
                    if (x)
                    {
                        toFavoriteCommand.Command = RemoveToDoItemFromFavoriteCommand;
                        toFavoriteCommand.Icon = MaterialIconKind.Star;
                    }
                    else
                    {
                        toFavoriteCommand.Command = AddToDoItemToFavoriteCommand;
                        toFavoriteCommand.Icon = MaterialIconKind.StarOutline;
                    }
                }
            );

        Commands.Add(toFavoriteCommand);
        Commands.Add(new(MaterialIconKind.Plus, AddToDoItemCommand));
        Commands.Add(new(MaterialIconKind.Leaf, ToLeafToDoItemsCommand));
        Commands.Add(new(MaterialIconKind.SwapHorizontal, ChangeRootItemCommand));
        Commands.Add(new(MaterialIconKind.FamilyTree, ToDoItemToRootCommand));
        Commands.Add(new(MaterialIconKind.CodeString, ToDoItemToStringCommand));
        Commands.Add(new(MaterialIconKind.Timer, AddTimerCommand));
    }

    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public AvaloniaList<ToDoItemChildrenType> ChildrenTypes { get; }
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

    [Inject]
    public required ToDoItemHeaderView ToDoItemHeaderView { get; init; }

    [Inject]
    public required ToDoSubItemsViewModel ToDoSubItemsViewModel { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathControl Path { get; set; }

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

    public abstract Task RefreshToDoItemAsync();

    private void AddTimer()
    {
        Navigator.NavigateTo<AddTimerViewModel>(
            vm =>
            {
                vm.EventId = EventIdHelper.ChangeFavoriteId;

                vm.Item = new()
                {
                    Id = Id,
                    Name = Name,
                };

                vm.DueDateTime = DateTimeOffset.Now.ToCurrentDay();
            }
        );
    }

    private Task ChangeDescriptionAsync()
    {
        return DialogViewer.ShowMultiStringConfirmDialogAsync(
            str =>
            {
                Description = str;

                return DialogViewer.CloseInputDialogAsync();
            },
            box =>
            {
                box.Text = Description;
                box.Watermark = "Description";
            }
        );
    }

    private void Initialized()
    {
        Commands.Add(new(MaterialIconKind.Checks, ToDoSubItemsViewModel.CompleteSelectedToDoItemsCommand));
        var viewModel = ToDoItemHeaderView.ViewModel.ThrowIfNull();
        viewModel.Item = this;
        viewModel.Commands.AddRange(Commands);
    }

    private Task ToDoItemToStringAsync()
    {
        return DialogViewer.ShowConfirmContentDialogAsync<ToDoItemToStringSettingsView>(
            async view =>
            {
                var statuses = view.ViewModel.ThrowIfNull().Statuses.Where(x => x.IsChecked).Select(x => x.Item);
                var options = new ToDoItemToStringOptions(statuses, Id);
                var text = await ToDoService.ToDoItemToStringAsync(options);
                await Clipboard.SetTextAsync(text);
                await DialogViewer.CloseContentDialogAsync();
            },
            _ => DialogViewer.CloseContentDialogAsync()
        );
    }

    private Task ChangeRootItemAsync()
    {
        return DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async item =>
            {
                await ToDoService.UpdateToDoItemParentAsync(Id, item.Id);
                await RefreshToDoItemAsync();
                await DialogViewer.CloseInputDialogAsync();
            },
            view =>
            {
                Path.Items ??= new();
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IgnoreIds.Add(Id);
                var parents = Path.Items.OfType<ToDoItemParentNotify>().ToArray();

                if (parents.Length == 1)
                {
                    return;
                }

                viewModel.DefaultSelectedItemId = parents[^2].Id;
            }
        );
    }

    private void ToLeafToDoItems()
    {
        Navigator.NavigateTo<LeafToDoItemsViewModel>(vm => vm.Id = Id);
    }

    private async Task RemoveToDoItemFromFavoriteAsync()
    {
        await ToDoService.RemoveFavoriteToDoItemAsync(Id);
        await RefreshToDoItemAsync();
    }

    private async Task AddToDoItemToCurrentAsync()
    {
        await ToDoService.AddFavoriteToDoItemAsync(Id);
        await RefreshToDoItemAsync();
    }

    private async Task ToDoItemToRootAsync()
    {
        await ToDoService.ToDoItemToRootAsync(Id);
        Navigator.NavigateTo<RootToDoItemsViewModel>();
    }

    protected async void OnNextId(Guid x)
    {
        await SafeExecuteAsync(RefreshToDoItemAsync);
    }

    protected async void OnNextName(string x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemNameAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    protected async void OnNextType(ToDoItemType x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemTypeAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }

    private Task AddToDoItemAsync()
    {
        return DialogViewer.ShowConfirmContentDialogAsync<AddToDoItemView>(
            async view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                var parentValue = viewModel.Parent.ThrowIfNull();
                var options = new AddToDoItemOptions(parentValue.Id, viewModel.Name);
                await ToDoService.AddToDoItemAsync(options);
                await ToDoService.NavigateToToDoItemViewModel(parentValue.Id, Navigator);
                await DialogViewer.CloseContentDialogAsync();
            },
            _ => DialogViewer.CloseContentDialogAsync(),
            view => view.ViewModel.ThrowIfNull().Parent = Mapper.Map<ToDoSubItemNotify>(this)
        );
    }

    private Task ChangeToDoItemByPathAsync(ToDoItemParentNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    private void ToRootItem()
    {
        Navigator.NavigateTo<RootToDoItemsViewModel>();
    }

    protected async void OnNextDescription(string x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateToDoItemDescriptionAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }
}