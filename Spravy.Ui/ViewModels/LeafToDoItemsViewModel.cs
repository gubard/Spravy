using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
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
        this.WhenAnyValue(x => x.Id).Skip(1).Subscribe(OnNextId);
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ICommand SwitchPaneCommand { get; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    public Guid Id
    {
        get => id;
        set => this.RaiseAndSetIfChanged(ref id, value);
    }

    private async void OnNextId(Guid x)
    {
        await CreateWithDialogProgressIndicatorAsync(RefreshToDoItemAsync).Invoke();
    }

    public async Task RefreshToDoItemAsync()
    {
        var items = await ToDoService.GetLeafToDoSubItemsAsync(Id);
        var notifyItems = Mapper.Map<IEnumerable<ToDoSubItemNotify>>(items);
        await ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItemsAsync(notifyItems, this);
    }

    private void SwitchPane()
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
    }
}