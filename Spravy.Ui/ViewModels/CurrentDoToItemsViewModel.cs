using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia;
using Avalonia.Collections;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using Spravy.Domain.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CurrentDoToItemsViewModel : RoutableViewModelBase
{
    public CurrentDoToItemsViewModel() : base("current-do-to-items")
    {
        InitializedCommand = CreateCommandFromTask(InitializedAsync);
        DeleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemNotify>(DeleteSubToDoItemAsync);
        CompleteSubToDoItemCommand = CreateCommandFromTask<ToDoSubItemValueNotify>(CompleteSubToDoItemAsync);
        ChangeToDoItemCommand = CreateCommand<ToDoSubItemNotify>(ChangeToDoItem);
        AddSubToDoItemToCurrentCommand = CreateCommandFromTask<ToDoSubItemNotify>(AddCurrentToDoItemAsync);
        RemoveSubToDoItemFromCurrentCommand = CreateCommandFromTask<ToDoSubItemNotify>(RemoveCurrentToDoItemAsync);
    }

    public ICommand CompleteSubToDoItemCommand { get; }
    public ICommand DeleteSubToDoItemCommand { get; }
    public ICommand ChangeToDoItemCommand { get; }
    public ICommand AddSubToDoItemToCurrentCommand { get; }
    public ICommand RemoveSubToDoItemFromCurrentCommand { get; }
    public ICommand InitializedCommand { get; }
    public AvaloniaList<ToDoSubItemNotify> Items { get; } = new();

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }

    private async Task RemoveCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.RemoveCurrentToDoItemAsync(item.Id);
        await RefreshToDoItemAsync();
    }

    private async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetCurrentToDoItemsAsync();
        Items.Clear();
        Items.AddRange(Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items));
    }

    private void ChangeToDoItem(ToDoSubItemNotify subItemValue)
    {
        Navigator.NavigateTo<ToDoItemValueViewModel>(vm => vm.Id = subItemValue.Id);
    }

    private async Task AddCurrentToDoItemAsync(ToDoSubItemNotify item)
    {
        await ToDoService.AddCurrentToDoItemAsync(item.Id);
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

    private async Task DeleteSubToDoItemAsync(ToDoSubItemNotify subItemValue)
    {
        await ToDoService.DeleteToDoItemAsync(subItemValue.Id);
        await RefreshToDoItemAsync();
    }
}