using Ninject;
using ReactiveUI;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ToDoItemsGroupByViewModel : ViewModelBase
{
    private readonly ToDoItemsGroupByStatusViewModel groupByStatus;

    public ToDoItemsGroupByViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }

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

    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

    [Reactive]
    public object? Content { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        Disposables.Add(
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
                )
        );

        return Result.AwaitableFalse;
    }

    public void ClearExcept(IEnumerable<Guid> ids)
    {
        GroupByNone.ClearExcept(ids);
        GroupByStatus.ClearExcept(ids);
        GroupByType.ClearExcept(ids);
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

    public void UpdateItem(Selected<ToDoItemNotify> item, bool updateOrder)
    {
        GroupByNone.UpdateItem(item, updateOrder);
        GroupByStatus.UpdateItem(item, updateOrder);
        GroupByType.UpdateItem(item, updateOrder);
    }
}