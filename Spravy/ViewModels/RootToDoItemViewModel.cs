using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using ExtensionFramework.AvaloniaUi.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Core.Interfaces;
using Spravy.Interfaces;
using Spravy.Models;

namespace Spravy.ViewModels;

public class RootToDoItemViewModel : RoutableViewModelBase, IItemsViewModel<ToDoItemNotify>, IToDoItemOrderChanger
{
    public RootToDoItemViewModel() : base("root-to-do-item")
    {
        InitializedCommand = CreateCommandFromTask(InitializedAsync);
        AddToDoItemCommand = CreateCommand(AddToDoItem);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoItemNotify>(DeleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoItemNotify>(ChangeToDoItem);
    }

    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public AvaloniaList<ToDoItemNotify> Items { get; } = new();

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
        Items.AddRange(items.Select(x => Mapper.Map<ToDoItemNotify>(x)).OrderBy(x => x.OrderIndex));
        SubscribeItems();
    }

    private async Task DeleteSubToDoItemAsync(ToDoItemNotify item)
    {
        await ToDoService.DeleteToDoItemAsync(item.Id);
        await InitializedAsync();
    }

    private void AddToDoItem()
    {
        Navigator.NavigateTo<AddRootToDoItemViewModel>();
    }

    private void ChangeToDoItem(ToDoItemNotify item)
    {
        Navigator.NavigateTo<ToDoItemViewModel>(vm => vm.Id = item.Id);
    }

    private void SubscribeItems()
    {
        foreach (var itemNotify in Items)
        {
            async void OnNextIsComplete(bool x)
            {
                if (ToDoService is null)
                {
                    return;
                }

                await ToDoService.UpdateCompleteStatusAsync(itemNotify.Id, x);
            }

            itemNotify.WhenAnyValue(x => x.IsComplete).Subscribe(OnNextIsComplete);
        }
    }
}