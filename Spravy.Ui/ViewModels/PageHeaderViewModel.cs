using Avalonia.Collections;
using Avalonia.Threading;
using ReactiveUI;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class PageHeaderViewModel : ViewModelBase
{
    private CommandItem? leftCommand;
    private CommandItem? rightCommand;
    private object? header;

    public CommandItem? LeftCommand
    {
        get => leftCommand;
        set => this.RaiseAndSetIfChanged(ref leftCommand, value);
    }

    public CommandItem? RightCommand
    {
        get => rightCommand;
        set => this.RaiseAndSetIfChanged(ref rightCommand, value);
    }

    public object? Header
    {
        get => header;
        set => this.RaiseAndSetIfChanged(ref header, value);
    }

    public AvaloniaList<CommandItem> Commands { get; } = new();

    public DispatcherOperation SetMultiCommands(ToDoSubItemsViewModel items)
    {
        return this.InvokeUIBackgroundAsync(
            () =>
            {
                Commands.Clear();

                Commands.Add(
                    CommandStorage.MultiCompleteToDoItemsItem.WithParam(
                        items.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                Commands.Add(
                    CommandStorage.MultiSetTypeToDoItemsItem.WithParam(
                        items.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                Commands.Add(
                    CommandStorage.MultiSetParentToDoItemsItem.WithParam(
                        items.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                Commands.Add(
                    CommandStorage.MultiMoveToDoItemsToRootItem.WithParam(
                        items.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
                Commands.Add(
                    CommandStorage.MultiDeleteToDoItemsItem.WithParam(
                        items.List.MultiToDoItems.GroupByNone.Items.Items
                    )
                );
            }
        );
    }
}