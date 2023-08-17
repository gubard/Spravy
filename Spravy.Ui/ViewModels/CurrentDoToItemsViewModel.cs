using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CurrentDoToItemsViewModel : RoutableViewModelBase, IRefreshToDoItem
{
    public CurrentDoToItemsViewModel() : base("current-do-to-items")
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
    }

    public ICommand InitializedCommand { get; }

    [Inject]
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetCurrentToDoItemsAsync();
        var notifyItems = Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items);
        ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItems(notifyItems, this);
    }
}