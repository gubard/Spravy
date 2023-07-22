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
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemViewModel : RoutableViewModelBase, IItemsViewModel<ToDoSubItemNotify>, IToDoItemOrderChanger
{
    public RootToDoItemViewModel() : base("root-to-do-item")
    {
        InitializedCommand = CreateCommandFromTask(InitializedAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoSubItemNotify>(ChangeToDoItem);
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemValueNotify>(CompleteSubToDoItemAsync);
        SearchCommand = CreateCommand(Search);
        ChangeParentToDoItemCommand = CreateCommandFromTask(ChangeParentToDoItemAsync);
    }

    public ICommand SearchCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand ChangeParentToDoItemCommand { get; }
    public AvaloniaList<ToDoSubItemNotify> Items { get; } = new();
    public AvaloniaList<ToDoSubItemNotify> CompletedItems { get; } = new();

    [Inject]
    public required IToDoService ToDoService { get; set; }

    [Inject]
    public required IMapper Mapper { get; set; }

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

    private void ChangeToDoItem(ToDoSubItemNotify subItemValue)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = subItemValue.Id);
    }

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
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

    private void Search()
    {
        Navigator.NavigateTo<SearchViewModel>();
    }

    private async Task AddToDoItemAsync()
    {
        await DialogViewer.ShowDialogAsync<AddRootToDoItemView>(v => v.ViewModel.ThrowIfNull().IsDialog = true);
    }

    private async Task ChangeParentToDoItemAsync()
    {
        var obj = await DialogViewer.ShowDialogAsync<RootToDoItemView>(view => view.ViewModel.ThrowIfNull().IsDialog = true);
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

            itemNotify.WhenAnyValue(x => x.IsComplete).Skip(1).Subscribe(OnNextIsComplete);
        }
    }
}