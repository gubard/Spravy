using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using Material.Icons;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemsViewModel : ViewModelBase
{
    private bool isMulti;
    private object? content;
    private readonly ToDoItemsGroupByViewModel toDoItems;
    private GroupBy groupBy = GroupBy.ByStatus;
    private readonly ToDoItemsGroupByViewModel multiToDoItems;
    private readonly ToDoItemsViewModel favorite;

    public MultiToDoItemsViewModel()
    {
        this.WhenAnyValue(x => x.IsMulti).Subscribe(x => Content = x ? MultiToDoItems : ToDoItems);

        this.WhenAnyValue(x => x.GroupBy)
            .Subscribe(
                x =>
                {
                    if (ToDoItems is null)
                    {
                        return;
                    }

                    if (MultiToDoItems is null)
                    {
                        return;
                    }

                    ToDoItems.GroupBy = x;
                    MultiToDoItems.GroupBy = x;
                }
            );
    }

    public AvaloniaList<GroupBy> GroupBys { get; } = new(Enum.GetValues<GroupBy>());

    [Inject]
    public required ToDoItemsViewModel Favorite
    {
        get => favorite;
        [MemberNotNull(nameof(favorite))]
        init
        {
            favorite = value;
            favorite.Header = "Favorite";
        }
    }

    [Inject]
    public required ToDoItemsGroupByViewModel ToDoItems
    {
        get => toDoItems;
        [MemberNotNull(nameof(toDoItems))]
        init
        {
            toDoItems = value;
            Content = toDoItems;
        }
    }

    [Inject]
    public required ToDoItemsGroupByViewModel MultiToDoItems
    {
        get => multiToDoItems;
        [MemberNotNull(nameof(multiToDoItems))]
        init
        {
            multiToDoItems = value;
            multiToDoItems.GroupByNone.Items.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByNone.Items.Items, c)).RunAsync
                    ),
                    "Select all",
                    null
                )
            );
            multiToDoItems.GroupByStatus.Missed.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByStatus.Missed.Items, c)).RunAsync
                    ),
                    "Select missed",
                    null
                )
            );
            multiToDoItems.GroupByStatus.Completed.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByStatus.Completed.Items, c)).RunAsync
                    ),
                    "Select completed",
                    null
                )
            );
            multiToDoItems.GroupByStatus.Planned.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByStatus.Planned.Items, c)).RunAsync
                    ),
                    "Select planned",
                    null
                )
            );
            multiToDoItems.GroupByStatus.ReadyForCompleted.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByStatus.ReadyForCompleted.Items, c))
                            .RunAsync
                    ),
                    "Select ready for completed",
                    null
                )
            );
            multiToDoItems.GroupByType.Groups.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.Groups.Items, c))
                            .RunAsync
                    ),
                    "Select groups",
                    null
                )
            );
            multiToDoItems.GroupByType.Circles.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.Circles.Items, c))
                            .RunAsync
                    ),
                    "Select circles",
                    null
                )
            );
            multiToDoItems.GroupByType.Periodicitys.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.Periodicitys.Items, c))
                            .RunAsync
                    ),
                    "Select periodicitys",
                    null
                )
            );
            multiToDoItems.GroupByType.Planneds.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.Planneds.Items, c))
                            .RunAsync
                    ),
                    "Select planneds",
                    null
                )
            );
            multiToDoItems.GroupByType.Steps.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.Steps.Items, c))
                            .RunAsync
                    ),
                    "Select steps",
                    null
                )
            );
            multiToDoItems.GroupByType.Values.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.Values.Items, c))
                            .RunAsync
                    ),
                    "Select values",
                    null
                )
            );
            multiToDoItems.GroupByType.PeriodicityOffsets.Commands.Add(
                new CommandItem(
                    MaterialIconKind.CheckAll,
                    CreateCommandFromTask(
                        TaskWork.Create(c => SelectAllAsync(multiToDoItems.GroupByType.PeriodicityOffsets.Items, c))
                            .RunAsync
                    ),
                    "Select periodicity offsets",
                    null
                )
            );
        }
    }

    public GroupBy GroupBy
    {
        get => groupBy;
        set => this.RaiseAndSetIfChanged(ref groupBy, value);
    }

    public bool IsMulti
    {
        get => isMulti;
        set => this.RaiseAndSetIfChanged(ref isMulti, value);
    }

    public object? Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public DispatcherOperation ClearAsync()
    {
        return this.InvokeUIBackgroundAsync(
            () =>
            {
                Favorite.Clear();
                ToDoItems.Clear();
                MultiToDoItems.Clear();
            }
        );
    }

    public DispatcherOperation AddItemsAsync(IEnumerable<ToDoItemNotify> items)
    {
        var selected = items.Select(x => new Selected<ToDoItemNotify>(x)).ToArray();

        return this.InvokeUIBackgroundAsync(
            () =>
            {
                ToDoItems.AddItems(selected);
                MultiToDoItems.AddItems(selected);
            }
        );
    }

    public DispatcherOperation AddFavoritesAsync(IEnumerable<ToDoItemNotify> items)
    {
        var selected = items.Select(x => new Selected<ToDoItemNotify>(x)).ToArray();

        return this.InvokeUIBackgroundAsync(() => Favorite.Items.AddRange(selected));
    }

    private async Task SelectAllAsync(AvaloniaList<Selected<ToDoItemNotify>> items, CancellationToken cancellationToken)
    {
        await this.InvokeUIAsync(
            () =>
            {
                if (items.All(x => x.IsSelect))
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = false;
                    }
                }
                else
                {
                    foreach (var item in items)
                    {
                        item.IsSelect = true;
                    }
                }
            }
        );
    }
}