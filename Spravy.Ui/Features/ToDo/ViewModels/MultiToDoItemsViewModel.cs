using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using Ninject;
using ReactiveUI;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

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
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByNone.Items.Items)
            );
            multiToDoItems.GroupByStatus.Missed.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.Missed.Items)
            );
            multiToDoItems.GroupByStatus.Completed.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.Completed.Items)
            );
            multiToDoItems.GroupByStatus.Planned.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.Planned.Items)
            );
            multiToDoItems.GroupByStatus.ReadyForCompleted.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByStatus.ReadyForCompleted.Items)
            );
            multiToDoItems.GroupByType.Groups.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Groups.Items)
            );
            multiToDoItems.GroupByType.Circles.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Circles.Items)
            );
            multiToDoItems.GroupByType.Periodicitys.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Periodicitys.Items)
            );
            multiToDoItems.GroupByType.Planneds.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Planneds.Items)
            );
            multiToDoItems.GroupByType.Steps.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Steps.Items)
            );
            multiToDoItems.GroupByType.Values.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.Values.Items)
            );
            multiToDoItems.GroupByType.PeriodicityOffsets.Commands.Add(
                CommandStorage.SelectAll.WithParam(multiToDoItems.GroupByType.PeriodicityOffsets.Items)
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