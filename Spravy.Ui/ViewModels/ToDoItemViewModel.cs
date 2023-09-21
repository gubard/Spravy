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
    private bool isCurrent;
    protected readonly List<ToDoItemCommand> Commands = new();

    public ToDoItemViewModel(string? urlPathSegment) : base(urlPathSegment)
    {
        RemoveToDoItemFromCurrentCommand =
            CreateCommandFromTaskWithDialogProgressIndicator(RemoveToDoItemFromCurrentAsync);

        var toCurrentCommand = new ToDoItemCommand(MaterialIconKind.Star, RemoveToDoItemFromCurrentCommand);
        ChangeDescriptionCommand = CreateCommandFromTask(ChangeDescriptionAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        ChangeToDoItemByPathCommand = CreateCommandFromTask<ToDoItemParentNotify>(ChangeToDoItemByPathAsync);
        ToRootItemCommand = CreateCommand(ToRootItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
        ChildrenTypes = new(Enum.GetValues<ToDoItemChildrenType>());
        AddToDoItemToCurrentCommand = CreateCommandFromTaskWithDialogProgressIndicator(AddToDoItemToCurrentAsync);
        ToLeafToDoItemsCommand = CreateCommand(ToLeafToDoItems);
        ChangeRootItemCommand = CreateCommandFromTask(ChangeRootItemAsync);
        ToDoItemToRootCommand = CreateCommandFromTask(ToDoItemToRootAsync);
        InitializedCommand = CreateCommand(Initialized);
        ToDoItemToStringCommand = CreateCommandFromTaskWithDialogProgressIndicator(ToDoItemToStringAsync);
        AddTimerCommand = CreateCommand(AddTimer);

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
    public ICommand AddToDoItemToCurrentCommand { get; }
    public ICommand RemoveToDoItemFromCurrentCommand { get; }
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

    private void AddTimer()
    {
        Navigator.NavigateTo<AddTimerViewModel>(
            vm =>
            {
                vm.EventId = EventIdHelper.ChangeCurrentId;

                vm.Item = new ToDoItemNotify
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
                DialogViewer.CloseDialog();
                Description = str;

                return Task.CompletedTask;
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

    private async Task ToDoItemToStringAsync()
    {
        var text = await ToDoService.ToDoItemToStringAsync(Id);
        await Clipboard.SetTextAsync(text);
    }

    private Task ChangeRootItemAsync()
    {
        return DialogViewer.ShowToDoItemSelectorConfirmDialogAsync(
            async item =>
            {
                await ToDoService.UpdateToDoItemParentAsync(Id, item.Id);
                await RefreshToDoItemAsync();
                DialogViewer.CloseDialog();
            },
            view =>
            {
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

    private Task AddToDoItemAsync()
    {
        return DialogViewer.ShowConfirmDialogAsync<AddToDoItemView>(
            _ =>
            {
                DialogViewer.CloseDialog();

                return Task.CompletedTask;
            },
            async view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
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

    private void ToRootItem()
    {
        Navigator.NavigateTo<RootToDoItemViewModel>();
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