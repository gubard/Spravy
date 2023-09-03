using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Controls;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class RootToDoItemViewModel : RoutableViewModelBase, IToDoItemOrderChanger, IRefreshToDoItem
{
    public RootToDoItemViewModel() : base("root-to-do-item")
    {
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        ToCurrentItemsCommand = CreateCommand(ToCurrentItems);
        SwitchPaneCommand = CreateCommand(SwitchPane);
    }

    public ICommand ToCurrentItemsCommand { get; }
    public ICommand InitializedCommand { get; }
    public ICommand AddToDoItemCommand { get; }
    public ICommand SwitchPaneCommand { get; }

    [Inject]
    public required ToDoSubItemsView ToDoSubItemsView { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required SplitView SplitView { get; init; }

    private void SwitchPane()
    {
        SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
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
        var items = await ToDoService.GetRootToDoSubItemsAsync();
        var source = items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
        ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItems(source, this);
        SubscribeItems(source);
    }

    private Task AddToDoItemAsync()
    {
        return DialogViewer.ShowConfirmDialogAsync<AddRootToDoItemView>(
            async view => DialogViewer.CloseDialog(),
            async view =>
            {
                var options = Mapper.Map<AddRootToDoItemOptions>(view.ViewModel);
                await ToDoService.AddRootToDoItemAsync(options);
                DialogViewer.CloseDialog();
                await RefreshToDoItemAsync();
            }
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
                        await ToDoService.UpdateToDoItemCompleteStatusAsync(itemNotify.Id, x);
                        await RefreshToDoItemAsync();
                    }
                );
            }

            itemNotify.WhenAnyValue(x => x.IsCompleted).Skip(1).Subscribe(OnNextIsComplete);
        }
    }
}