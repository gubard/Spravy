using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Avalonia.Input.Platform;
using Material.Icons;
using ReactiveUI;
using Spravy.Domain.Attributes;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Controls;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public abstract class ToDoItemViewModel : RoutableViewModelBase,
    IToDoItemOrderChanger
{
    private string name = string.Empty;
    private Guid id;
    private string description = string.Empty;

    protected readonly List<IDisposable> PropertySubscribes = new();
    private ToDoItemType type;
    private bool isCurrent;
    protected readonly List<ToDoItemCommand> Commands = new();
    private readonly ToDoItemCommand toCurrentCommand;

    public ToDoItemViewModel(string? urlPathSegment) : base(urlPathSegment)
    {
        toCurrentCommand = new(MaterialIconKind.Star, RemoveToDoItemFromCurrentCommand);

        AddSubToDoItemToCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(AddCurrentToDoItemAsync);
        RemoveSubToDoItemFromCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(RemoveCurrentToDoItemAsync);
        CompleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        DeleteSubToDoItemCommand =
            CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItemAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        ChangeToDoItemByPathCommand = CreateCommandFromTask<ToDoItemParentNotify>(ChangeToDoItemByPathAsync);
        ToRootItemCommand = CreateCommand(ToRootItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
        SearchCommand = CreateCommand(Search);
        AddToDoItemToCurrentCommand = CreateCommandFromTaskWithDialogProgressIndicator(AddToDoItemToCurrentAsync);
        RemoveToDoItemFromCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator(RemoveToDoItemFromCurrentAsync);
        ToCurrentItemsCommand = CreateCommand(ToCurrentItems);
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItemAsync);
        ToLeafToDoItemsCommand = CreateCommand(ToLeafToDoItems);
        ChangeRootItemCommand = CreateCommandFromTask(ChangeRootItemAsync);
        ToDoItemToRootCommand = CreateCommandFromTask(ToDoItemToRootAsync);
        InitializedCommand = CreateCommand(Initialized);
        ToDoItemToStringCommand = CreateCommandFromTaskWithDialogProgressIndicator(ToDoItemToStringAsync);

        this.WhenAnyValue(x => x.IsCurrent)
            .Subscribe(
                x =>
                {
                    if (x)
                    {
                        toCurrentCommand.Command = RemoveToDoItemFromCurrentCommand;
                        toCurrentCommand.Icon = MaterialIconKind.Star;
                    }
                    else
                    {
                        toCurrentCommand.Command = AddToDoItemToCurrentCommand;
                        toCurrentCommand.Icon = MaterialIconKind.StarOutline;
                    }
                }
            );

        Commands.Add(toCurrentCommand);
        Commands.Add(new(MaterialIconKind.Plus, AddToDoItemCommand));
        Commands.Add(new(MaterialIconKind.Search, SearchCommand));
        Commands.Add(new(MaterialIconKind.Creation, ToCurrentItemsCommand));
        Commands.Add(new(MaterialIconKind.Leaf, ToLeafToDoItemsCommand));
        Commands.Add(new(MaterialIconKind.SwapHorizontal, ChangeRootItemCommand));
        Commands.Add(new(MaterialIconKind.FamilyTree, ToDoItemToRootCommand));
        Commands.Add(new(MaterialIconKind.CodeString, ToDoItemToStringCommand));
    }

    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public ICommand AddSubToDoItemToCurrentCommand { get; }
    public ICommand RemoveSubToDoItemFromCurrentCommand { get; }
    public ICommand AddToDoItemToCurrentCommand { get; }
    public ICommand RemoveToDoItemFromCurrentCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand ChangeToDoItemByPathCommand { get; }
    public ICommand ToRootItemCommand { get; }
    public ICommand ToCurrentItemsCommand { get; }
    public ICommand ToLeafToDoItemsCommand { get; }
    public ICommand ChangeRootItemCommand { get; }
    public ICommand ToDoItemToRootCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand ToDoItemToStringCommand { get; }

    [Inject]
    public required ToDoItemHeaderView ToDoItemHeaderView { get; init; }

    [Inject]
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathControl Path { get; set; }

    [Inject]
    public required IClipboard Clipboard { get; set; }

    public bool IsCurrent
    {
        get => isCurrent;
        set => this.RaiseAndSetIfChanged(ref isCurrent, value);
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

    private void Initialized()
    {
        var viewModel = ToDoItemHeaderView.ViewModel.ThrowIfNull();
        viewModel.Item = this;
        viewModel.Commands.AddRange(Commands);
    }

    private async Task ToDoItemToStringAsync()
    {
        var text = await ToDoService.ToDoItemToStringAsync(Id);
        await Clipboard.SetTextAsync(text);
    }

    private Task ChangeRootItemAsync()
    {
        return DialogViewer.ShowConfirmDialogAsync<ToDoItemSelectorView>(
            async _ => DialogViewer.CloseDialog(),
            async view =>
            {
                await ToDoService.UpdateToDoItemParentAsync(Id, view.ViewModel.SelectedItem.Id);
                await RefreshToDoItemAsync();
                DialogViewer.CloseDialog();
            }
        );
    }

    private void ToLeafToDoItems()
    {
        Navigator.NavigateTo<LeafToDoItemsViewModel>(vm => vm.Id = Id);
    }

    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    private void ToCurrentItems()
    {
        Navigator.NavigateTo<CurrentDoToItemsViewModel>();
    }

    private async Task RemoveToDoItemFromCurrentAsync()
    {
        await ToDoService.RemoveCurrentToDoItemAsync(Id);
        await RefreshToDoItemAsync();
    }

    private async Task AddToDoItemToCurrentAsync()
    {
        await ToDoService.AddCurrentToDoItemAsync(Id);
        await RefreshToDoItemAsync();
    }

    private async Task RemoveCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.RemoveCurrentToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private async Task AddCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.AddCurrentToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private void Search()
    {
        Navigator.NavigateTo<SearchViewModel>();
    }

    private async Task ToDoItemToRootAsync()
    {
        await ToDoService.ToDoItemToRootAsync(Id);
        Navigator.NavigateTo<RootToDoItemViewModel>();
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

    private async Task CompleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await DialogViewer.ShowDialogAsync<CompleteToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.Item = subItemValue;
            }
        );

        await RefreshToDoItemAsync();
    }

    private Task AddToDoItemAsync()
    {
        return DialogViewer.ShowConfirmDialogAsync<AddToDoItemView>(
            async _ => DialogViewer.CloseDialog(),
            async view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                var parentValue = viewModel.Parent.ThrowIfNull();
                var options = new AddToDoItemOptions(parentValue.Id, viewModel.Name);
                await ToDoService.AddToDoItemAsync(options);
                await ToDoService.NavigateToToDoItemViewModel(parentValue.Id, Navigator);
                DialogViewer.CloseDialog();
            },
            view => view.ViewModel.ThrowIfNull().Parent = Mapper.Map<ToDoSubItemNotify>(this)
        );
    }

    private Task ChangeToDoItemByPathAsync(ToDoItemParentNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    private Task ChangeToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private void ToRootItem()
    {
        Navigator.NavigateTo<RootToDoItemViewModel>();
    }

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
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