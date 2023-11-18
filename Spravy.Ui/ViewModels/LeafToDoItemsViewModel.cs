using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class LeafToDoItemsViewModel : RoutableViewModelBase, IRefreshToDoItem
{
    private Guid id;

    public LeafToDoItemsViewModel() : base("leaf-to-do-items")
    {
        SwitchPaneCommand = CreateCommand(SwitchPane);
        InitializedCommand = CreateInitializedCommand(InitializedAsync);
    }

    public ICommand InitializedCommand { get; }
    public ICommand SwitchPaneCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required MainSplitViewModel MainSplitViewModel { get; init; }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetLeafToDoSubItemsAsync(Id).ConfigureAwait(false);
        var notifyItems = Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items);
        await ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItemsAsync(notifyItems, this);
    }

    private void SwitchPane()
    {
        MainSplitViewModel.IsPaneOpen = !MainSplitViewModel.IsPaneOpen;
    }

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }
}