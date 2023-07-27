using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.AvaloniaUi.Interfaces;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemViewModel : RoutableViewModelBase,
    IItemsViewModel<ToDoSubItemNotify>,
    IToDoItemOrderChanger
{
    public RootToDoItemViewModel() : base("root-to-do-item")
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(ChangeToDoItem);
        CompleteSubToDoItemCommand = CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(CompleteSubToDoItemAsync);
        SearchCommand = CreateCommand(Search);
        ChangeParentToDoItemCommand = CreateCommandFromTask(ChangeParentToDoItemAsync);
        AddSubToDoItemToCurrentCommand = CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(AddCurrentToDoItemAsync);
        RemoveSubToDoItemFromCurrentCommand = CreateCommandFromTaskWithDialogProgressIndicator<ToDoSubItemNotify>(RemoveCurrentToDoItemAsync);
        ToCurrentItemsCommand = CreateCommand(ToCurrentItems);
        ChangeToActiveDoItemCommand = CreateCommandFromTask<ActiveToDoItemNotify>(ChangeToActiveDoItemAsync);
    }

    public ICommand ToCurrentItemsCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand ChangeParentToDoItemCommand { get; }
    public ICommand AddSubToDoItemToCurrentCommand { get; }
    public ICommand RemoveSubToDoItemFromCurrentCommand { get; }
    public ICommand ChangeToActiveDoItemCommand { get; }
    public AvaloniaList<ToDoSubItemNotify> Items { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> CompletedItems { get; } = new();

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

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

    private void ToCurrentItems()
    {
        Navigator.NavigateTo<CurrentDoToItemsViewModel>();
    }

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetRootToDoItemsAsync();
        Items.Clear();
        CompletedItems.Clear();
        var source = items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
        Items.AddRange(source.Where(x => x.Status != ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
        CompletedItems.AddRange(source.Where(x => x.Status == ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
        SubscribeItems(Items);
        SubscribeItems(CompletedItems);
    }
    
    private Task ChangeToActiveDoItemAsync(ActiveToDoItemNotify item)
    {
        return ToDoService.NavigateToToDoItemViewModel(item.Id, Navigator);
    }

    private Task ChangeToDoItem(ToDoSubItemNotify subItemValue)
    {
        return ToDoService.NavigateToToDoItemViewModel(subItemValue.Id, Navigator);
    }

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
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

    private void Search()
    {
        Navigator.NavigateTo<SearchViewModel>();
    }

    private Task AddToDoItemAsync()
    {
        return DialogViewer.ShowDialogAsync<AddRootToDoItemView>(v => v.ViewModel.ThrowIfNull().IsDialog = true);
    }

    private Task ChangeParentToDoItemAsync()
    {
        return DialogViewer.ShowDialogAsync<RootToDoItemView>(
            view => view.ViewModel.ThrowIfNull().IsDialog = true
        );
    }

    private void SubscribeItems(IEnumerable<ToDoSubItemNotify> items)
    {
        foreach (var itemNotify in items.OfType<ToDoSubItemValueNotify>())
        {
            async void OnNextIsComplete(bool x)
            {
                await SafeExecuteAsync(
                    async () =>
                    {
                        await ToDoService.UpdateCompleteStatusAsync(itemNotify.Id, x);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsComplete);
        }
    }
}