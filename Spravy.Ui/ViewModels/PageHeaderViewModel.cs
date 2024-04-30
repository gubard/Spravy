using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia.Collections;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.ViewModels;

public class PageHeaderViewModel : ViewModelBase
{
    [Reactive]
    public CommandItem? LeftCommand { get; set; }

    [Reactive]
    public CommandItem? RightCommand { get; set; }

    [Reactive]
    public object? Header { get; set; }

    public AvaloniaList<CommandItem> Commands { get; } = new();

    public ConfiguredValueTaskAwaitable<Result> SetMultiCommands(ToDoSubItemsViewModel items)
    {
        return this.InvokeUIBackgroundAsync(() =>
        {
            Commands.Clear();

            Commands.Add(
                CommandStorage.MultiCompleteToDoItemsItem.WithParam(items.List.MultiToDoItems.GroupByNone.Items.Items));

            Commands.Add(
                CommandStorage.MultiSetTypeToDoItemsItem.WithParam(items.List.MultiToDoItems.GroupByNone.Items.Items));

            Commands.Add(
                CommandStorage.MultiSetParentToDoItemsItem.WithParam(items.List.MultiToDoItems.GroupByNone.Items
                   .Items));

            Commands.Add(
                CommandStorage.MultiMoveToDoItemsToRootItem.WithParam(items.List.MultiToDoItems.GroupByNone.Items
                   .Items));

            Commands.Add(
                CommandStorage.MultiDeleteToDoItemsItem.WithParam(items.List.MultiToDoItems.GroupByNone.Items.Items));

            Disposables.AddRange(Commands.Select(x => (IDisposable)x.Command));
        });
    }
}