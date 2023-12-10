using Ninject;
using ReactiveUI;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByViewModel : ViewModelBase
{
    private GroupBy groupBy = GroupBy.ByStatus;
    private object? content;
    private readonly ToDoItemsGroupByStatusViewModel groupByStatus;

    public ToDoItemsGroupByViewModel()
    {
        this.WhenAnyValue(x => x.GroupBy)
            .Subscribe(
                x =>
                {
                    Content = x switch
                    {
                        GroupBy.None => GroupByNone,
                        GroupBy.ByStatus => GroupByStatus,
                        GroupBy.ByType => GroupByType,
                        _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
                    };
                }
            );
    }

    [Inject]
    public required ToDoItemsGroupByNoneViewModel GroupByNone { get; init; }

    [Inject]
    public required ToDoItemsGroupByStatusViewModel GroupByStatus
    {
        get => groupByStatus;
        [MemberNotNull(nameof(groupByStatus))]
        init
        {
            groupByStatus = value;
            Content = groupByStatus;
        }
    }

    [Inject]
    public required ToDoItemsGroupByTypeViewModel GroupByType { get; init; }

    public GroupBy GroupBy
    {
        get => groupBy;
        set => this.RaiseAndSetIfChanged(ref groupBy, value);
    }

    public object? Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public void Clear()
    {
        GroupByNone.Clear();
        GroupByStatus.Clear();
        GroupByType.Clear();
    }

    public void AddItems(IEnumerable<Selected<ToDoItemNotify>> items)
    {
        GroupByNone.AddItems(items);
        GroupByStatus.AddItems(items);
        GroupByType.AddItems(items);
    }
}