using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.AvaloniaUi.Interfaces;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public abstract class ToDoItemViewModel : RoutableViewModelBase,
    IItemsViewModel<ToDoSubItemNotify>,
    IToDoItemOrderChanger
{
    private string name = string.Empty;
    private Guid id;
    private string description = string.Empty;

    protected readonly List<IDisposable> PropertySubscribes = new();
    private ToDoItemType type;
    private bool isCurrent;

    public ToDoItemViewModel(string? urlPathSegment) : base(urlPathSegment)
    {
        AddSubToDoItemToCurrentCommand = CreateCommandFromTask<ToDoSubItemNotify>(AddCurrentToDoItemAsync);
        RemoveSubToDoItemFromCurrentCommand = CreateCommandFromTask<ToDoSubItemNotify>(RemoveCurrentToDoItemAsync);
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemValueNotify>(CompleteSubToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoSubItemNotify>(ChangeToDoItem);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        ChangeToDoItemByPathCommand = CreateCommand<ToDoItemParentNotify>(ChangeToDoItemByPath);
        ToRootItemCommand = CreateCommand(ToRootItem);
        TypeOfPeriodicities = new(Enum.GetValues<TypeOfPeriodicity>());
        ToDoItemTypes = new(Enum.GetValues<ToDoItemType>());
        SearchCommand = CreateCommand(Search);
        AddToDoItemToCurrentCommand = CreateCommandFromTask(AddToDoItemToCurrentAsync);
        RemoveToDoItemFromCurrentCommand = CreateCommandFromTask(RemoveToDoItemFromCurrentAsync);
        ToCurrentItemsCommand = CreateCommand(ToCurrentItems);
        ChangeToActiveDoItemCommand = CreateCommand<ActiveToDoItemNotify>(ChangeToActiveDoItem);
    }

    public AvaloniaList<ToDoSubItemNotify> CompletedItems { get; } = new();
    public AvaloniaList<TypeOfPeriodicity> TypeOfPeriodicities { get; }
    public AvaloniaList<ToDoItemType> ToDoItemTypes { get; }
    public AvaloniaList<ToDoSubItemNotify> Items { get; } = new();
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

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

    [Inject]
    public required PathControl Path { get; set; }

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
    
    private void ChangeToActiveDoItem(ActiveToDoItemNotify item)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = item.Id);
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

    protected async void OnNextId(Guid x)
    {
        await SafeExecuteAsync(RefreshToDoItemAsync);
    }

    protected async void OnNextName(string x)
    {
        await SafeExecuteAsync(
            async () =>
            {
                await ToDoService.UpdateNameToDoItemAsync(Id, x);
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

    private async Task CompleteSubToDoItemAsync(ToDoSubItemValueNotify subItemValue)
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

    private async Task AddToDoItemAsync()
    {
        await DialogViewer.ShowDialogAsync<AddToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.ThrowIfNull().Parent = Mapper.Map<ToDoSubItemNotify>(this);
            }
        );
    }

    private void ChangeToDoItemByPath(ToDoItemParentNotify item)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = item.Id);
    }

    private void ChangeToDoItem(ToDoSubItemNotify subItemValue)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = subItemValue.Id);
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
                await ToDoService.UpdateDescriptionToDoItemAsync(Id, x);
                await RefreshToDoItemAsync();
            }
        );
    }
}