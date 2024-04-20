using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using AutoMapper;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Errors;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Errors;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Features.Localizations.Models;
using Spravy.Ui.Features.ToDo.Enums;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemsViewModel : ViewModelBase
{
    private readonly ToDoItemsGroupByViewModel toDoItems;
    private readonly ToDoItemsGroupByViewModel multiToDoItems;
    private readonly ToDoItemsViewModel favorite;

    public MultiToDoItemsViewModel()
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    public AvaloniaList<GroupBy> GroupBys { get; } = new(Enum.GetValues<GroupBy>());

    [Inject]
    public required IMapper Mapper { get; init; }

    [Inject]
    public required IToDoService ToDoService { get; init; }

    [Inject]
    public required ToDoItemsViewModel Favorite
    {
        get => favorite;
        [MemberNotNull(nameof(favorite))]
        init
        {
            favorite?.Dispose();
            favorite = value;
            favorite.Header = new TextView("MultiToDoItemsView.Favorite");
            Disposables.Add(favorite);
        }
    }

    [Inject]
    public required ToDoItemsGroupByViewModel ToDoItems
    {
        get => toDoItems;
        [MemberNotNull(nameof(toDoItems))]
        init
        {
            favorite?.Dispose();
            toDoItems = value;
            Content = toDoItems;
            Disposables.Add(favorite);
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

    [Reactive]
    public GroupBy GroupBy { get; set; } = GroupBy.ByStatus;

    [Reactive]
    public bool IsMulti { get; set; }

    [Reactive]
    public object? Content { get; set; }

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        Disposables.Add(this.WhenAnyValue(x => x.IsMulti).Subscribe(x => Content = x ? MultiToDoItems : ToDoItems));

        Disposables.Add(
            this.WhenAnyValue(x => x.GroupBy)
                .Subscribe(
                    x =>
                    {
                        ToDoItems.GroupBy = x;
                        MultiToDoItems.GroupBy = x;
                    }
                )
        );

        return Result.AwaitableFalse;
    }

    public ConfiguredValueTaskAwaitable<Result> ClearFavoriteExceptAsync(IEnumerable<Guid> ids)
    {
        return this.InvokeUIBackgroundAsync(() => Favorite.ClearExcept(ids));
    }

    public ConfiguredValueTaskAwaitable<Result> ClearExceptAsync(IEnumerable<Guid> ids)
    {
        return this.InvokeUIBackgroundAsync(
            () =>
            {
                ToDoItems.ClearExcept(ids);
                MultiToDoItems.ClearExcept(ids);
            }
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateFavoriteItemAsync(ToDoItem item)
    {
        return this.InvokeUIBackgroundAsync(
            () =>
            {
                var notify = Favorite.Items.SingleOrDefault(x => x.Value.Id == item.Id)
                             ?? new Selected<ToDoItemNotify>(Mapper.Map<ToDoItemNotify>(item));

                var updateOrder = item.OrderIndex != notify.Value.OrderIndex;
                SetupItem(notify.Value, item);
                Favorite.UpdateItem(notify, updateOrder);
            }
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateItemAsync(ToDoItem item)
    {
        return this.InvokeUIBackgroundAsync(
            () =>
            {
                var notify = ToDoItems.GroupByNone.Items.Items.SingleOrDefault(x => x.Value.Id == item.Id)
                             ?? new Selected<ToDoItemNotify>(Mapper.Map<ToDoItemNotify>(item));

                var updateOrder = item.OrderIndex != notify.Value.OrderIndex;
                SetupItem(notify.Value, item);
                ToDoItems.UpdateItem(notify, updateOrder);
                MultiToDoItems.UpdateItem(notify, updateOrder);
            }
        );
    }

    private ToDoItemNotify SetupItem(ToDoItemNotify notify, ToDoItem item)
    {
        notify.Type = item.Type;
        notify.Status = item.Status;
        notify.Active = Mapper.Map<ActiveToDoItemNotify?>(item.Active);
        notify.Description = item.Description;
        notify.Link = item.Link?.AbsoluteUri ?? string.Empty;
        notify.Name = item.Name;
        notify.IsCan = item.IsCan;
        notify.IsFavorite = item.IsFavorite;
        notify.ParentId = item.ParentId;
        notify.OrderIndex = item.OrderIndex;
        SetupItemCommands(notify);

        return notify;
    }

    private ToDoItemNotify SetupItemCommands(ToDoItemNotify item)
    {
        item.Commands.Clear();
        var toFavoriteCommand = CommandStorage.AddToDoItemToFavoriteItem.WithParam(item.Id);
        item.Commands.Add(CommandStorage.AddToDoItemChildItem.WithParam(item));
        item.Commands.Add(CommandStorage.DeleteToDoItemItem.WithParam(item));
        item.Commands.Add(CommandStorage.ShowToDoSettingItem.WithParam(item));
        item.Commands.Add(CommandStorage.CloneToDoItemItem.WithParam(item));

        if (item.IsFavorite)
        {
            toFavoriteCommand = CommandStorage.RemoveToDoItemFromFavoriteItem.WithParam(item.Id);
        }

        item.Commands.Add(toFavoriteCommand);
        item.Commands.Add(CommandStorage.NavigateToLeafItem.WithParam(item.Id));
        item.Commands.Add(CommandStorage.SetToDoParentItemItem.WithParam(item));
        item.Commands.Add(CommandStorage.MoveToDoItemToRootItem.WithParam(item));
        item.Commands.Add(CommandStorage.ToDoItemToStringItem.WithParam(item));
        item.Commands.Add(CommandStorage.ToDoItemRandomizeChildrenOrderIndexItem.WithParam(item));
        item.Commands.Add(CommandStorage.ChangeOrderIndexItem.WithParam(item));
        item.Commands.Add(CommandStorage.ResetToDoItemItem.WithParam(item));

        item.CompleteCommand =
            CreateCommandFromTask<ICanCompleteProperty>(
                TaskWork.Create<ICanCompleteProperty>(SwitchCompleteToDoItemAsync).RunAsync
            );

        return item;
    }

    private ConfiguredValueTaskAwaitable<Result> SwitchCompleteToDoItemAsync(
        ICanCompleteProperty property,
        CancellationToken cancellationToken
    )
    {
        return this.InvokeUIBackgroundAsync(() => property.IsBusy = true)
            .IfSuccessTryFinallyAsync(
                () => SwitchCompleteToDoItemCore(property, cancellationToken),
                () => this.InvokeUIBackgroundAsync(() => property.IsBusy = false)
                    .ToValueTask()
                    .ConfigureAwait(false),
                cancellationToken
            );
    }

    private ConfiguredValueTaskAwaitable<Result> SwitchCompleteToDoItemCore(
        ICanCompleteProperty property,
        CancellationToken cancellationToken
    )
    {
        switch (property.IsCan)
        {
            case ToDoItemIsCan.None:
                return Result.AwaitableFalse;
            case ToDoItemIsCan.CanComplete:
                return ToDoService.UpdateToDoItemCompleteStatusAsync(
                        property.Id,
                        true,
                        cancellationToken
                    )
                    .IfSuccessAsync(() => CommandStorage.RefreshCurrentViewAsync(cancellationToken), cancellationToken);
            case ToDoItemIsCan.CanIncomplete:
                return ToDoService.UpdateToDoItemCompleteStatusAsync(
                        property.Id,
                        false,
                        cancellationToken
                    )
                    .IfSuccessAsync(() => CommandStorage.RefreshCurrentViewAsync(cancellationToken), cancellationToken);
            default:
                return new Result(new ToDoItemIsCanOutOfRangeError(property.IsCan)).ToValueTaskResult().ConfigureAwait(false);
        }
    }
}