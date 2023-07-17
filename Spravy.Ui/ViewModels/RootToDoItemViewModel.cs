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
using ExtensionFramework.Core.Ui.Interfaces;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Enums;
using Spravy.Core.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemViewModel : RoutableViewModelBase, IItemsViewModel<ToDoItemNotify>, IToDoItemOrderChanger
{
    public RootToDoItemViewModel() : base("root-to-do-item")
    {
        InitializedCommand = CreateCommandFromTask(InitializedAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoItemNotify>(ChangeToDoItem);
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(CompleteSubToDoItemAsync);
    }

    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public AvaloniaList<ToDoItemNotify> Items { get; } = new();
    public AvaloniaList<ToDoItemNotify> CompletedItems { get; } = new();
    public ICommand CompleteSubToDoItemCommand { get; }

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
        var source = items.Select(x => Mapper.Map<ToDoItemNotify>(x)).ToArray();
        Items.AddRange(source.Where(x => x.Status != ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
        CompletedItems.AddRange(source.Where(x => x.Status == ToDoItemStatus.Complete).OrderBy(x => x.OrderIndex));
        SubscribeItems(Items);
        SubscribeItems(CompletedItems);
    }

    private async Task DeleteSubToDoItemAsync(ToDoItemNotify item)
    {
        await ToDoService.DeleteToDoItemAsync(item.Id);
        await InitializedAsync();
    }

    private async Task CompleteSubToDoItemAsync(ToDoItemNotify item)
    {
        await DialogViewer.ShowDialogAsync<CompleteToDoItemView>(
            view =>
            {
                var viewModel = view.ViewModel.ThrowIfNull();
                viewModel.IsDialog = true;
                viewModel.Item = item;
            }
        );

        await RefreshToDoItemAsync();
    }

    private async Task AddToDoItemAsync()
    {
        await DialogViewer.ShowDialogAsync<AddRootToDoItemView>(v => v.ViewModel.ThrowIfNull().IsDialog = true);
    }

    private void ChangeToDoItem(ToDoItemNotify item)
    {
        Navigator.NavigateTo<ToDoItemViewModel>(vm => vm.Id = item.Id);
    }

    private void SubscribeItems(IEnumerable<ToDoItemNotify> items)
    {
        foreach (var itemNotify in items)
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