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
using Serilog;
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
        Log.Logger.Information("Test {Number}", 15);
        InitializedCommand = CreateCommandFromTaskWithDialogProgressIndicator(InitializedAsync);
        Log.Logger.Information("Test {Number}", 16);
        AddToDoItemCommand = CreateCommandFromTask(AddToDoItemAsync);
        Log.Logger.Information("Test {Number}", 17);
        SwitchPaneCommand = CreateCommand(SwitchPane);
        Log.Logger.Information("Test {Number}", 18);
    }

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

    private Task InitializedAsync()
    {
        return RefreshToDoItemAsync();
    }

    public async Task RefreshToDoItemAsync()
    {
        Log.Logger.Information("Test {Number}", 5);
        var items = await ToDoService.GetRootToDoSubItemsAsync();
        Log.Logger.Information("Test {Number}", 6);
        var source = items.Select(x => Mapper.Map<ToDoSubItemNotify>(x)).ToArray();
        Log.Logger.Information("Test {Number}", 7);
        await ToDoSubItemsView.ViewModel.ThrowIfNull().UpdateItemsAsync(source, this);
        Log.Logger.Information("Test {Number}", 8);
        SubscribeItems(source);
        Log.Logger.Information("Test {Number}", 9);
    }

    private Task AddToDoItemAsync()
    {
        return DialogViewer.ShowConfirmContentDialogAsync<AddRootToDoItemView>(
            async view =>
            {
                var options = Mapper.Map<AddRootToDoItemOptions>(view.ViewModel);
                await ToDoService.AddRootToDoItemAsync(options);
                await DialogViewer.CloseContentDialogAsync();
                await RefreshToDoItemAsync();
            },
            _ => DialogViewer.CloseContentDialogAsync()
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