using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.ToDo.Service.Services;

[Authorize]
public class GrpcToDoService : ToDoService.ToDoServiceBase
{
    private readonly IToDoService toDoService;
    private readonly IConverter converter;
    private readonly ISerializer serializer;

    public GrpcToDoService(IToDoService toDoService, IConverter converter, ISerializer serializer)
    {
        this.toDoService = toDoService;
        this.converter = converter;
        this.serializer = serializer;
    }

    public override Task<CloneToDoItemReply> CloneToDoItem(
        CloneToDoItemRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.CloneId)
            .IfSuccessAsync(
                converter.Convert<Guid?>(request.ParentId),
                (ci, pi) => toDoService.CloneToDoItemAsync(ci, pi, context.CancellationToken)
            )
            .HandleAsync<CloneToDoItemReply>(serializer);
    }

    public override Task<GetChildrenToDoItemShortsReply> GetChildrenToDoItemShorts(
        GetChildrenToDoItemShortsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                i => toDoService.GetChildrenToDoItemShortsAsync(i, context.CancellationToken)
                    .IfSuccessAsync(items => converter.Convert<ToDoShortItemGrpc[]>(items))
            )
            .HandleAsync(
                serializer,
                items =>
                {
                    var result = new GetChildrenToDoItemShortsReply();
                    result.Items.AddRange(items);

                    return result;
                }
            );
    }

    public override Task<UpdateToDoItemDescriptionTypeReply> UpdateToDoItemDescriptionType(
        UpdateToDoItemDescriptionTypeRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                i => toDoService.UpdateToDoItemDescriptionTypeAsync(
                    i,
                    (DescriptionType)request.Type,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemDescriptionTypeReply>(serializer);
    }

    public override Task<ResetToDoItemReply> ResetToDoItem(
        ResetToDoItemRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.ResetToDoItemAsync(i, context.CancellationToken))
            .HandleAsync<ResetToDoItemReply>(serializer);
    }

    public override Task<GetTodayToDoItemsReply> GetTodayToDoItems(
        GetTodayToDoItemsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetTodayToDoItemsAsync(context.CancellationToken)
            .IfSuccessAsync(ids => converter.Convert<ByteString[]>(ids.ToArray()))
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetTodayToDoItemsReply();
                    reply.Ids.AddRange(ids);

                    return reply;
                }
            );
    }

    public override Task<UpdateToDoItemIsRequiredCompleteInDueDateReply>
        UpdateToDoItemIsRequiredCompleteInDueDate(
            UpdateToDoItemIsRequiredCompleteInDueDateRequest request,
            ServerCallContext context
        )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                i => toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                    i,
                    request.IsRequiredCompleteInDueDate,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemIsRequiredCompleteInDueDateReply>(serializer);
    }

    public override async Task GetToDoItems(
        GetToDoItemsRequest request,
        IServerStreamWriter<GetToDoItemsReply> responseStream,
        ServerCallContext context
    )
    {
        var ids = converter.Convert<Guid[]>(request.Ids).Value;

        await foreach (var item in toDoService.GetToDoItemsAsync(ids, request.ChunkSize, context.CancellationToken))
        {
            var reply = new GetToDoItemsReply();
            reply.Items.AddRange(converter.Convert<ToDoItemGrpc[]>(item.ToArray()).Value);
            await responseStream.WriteAsync(reply);
        }
    }

    public override Task<RandomizeChildrenOrderIndexReply> RandomizeChildrenOrderIndex(
        RandomizeChildrenOrderIndexRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.RandomizeChildrenOrderIndexAsync(i, context.CancellationToken))
            .HandleAsync<RandomizeChildrenOrderIndexReply>(serializer);
    }

    public override Task<GetPeriodicityOffsetToDoItemSettingsReply> GetPeriodicityOffsetToDoItemSettings(
        GetPeriodicityOffsetToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetPeriodicityOffsetToDoItemSettingsAsync(i, context.CancellationToken))
            .IfSuccessAsync(s => converter.Convert<GetPeriodicityOffsetToDoItemSettingsReply>(s))
            .HandleAsync(serializer);
    }

    public override Task<GetMonthlyPeriodicityReply> GetMonthlyPeriodicity(
        GetMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetMonthlyPeriodicityAsync(i, context.CancellationToken))
            .IfSuccessAsync(p => converter.Convert<GetMonthlyPeriodicityReply>(p))
            .HandleAsync(serializer, p => p);
    }

    public override Task<GetWeeklyPeriodicityReply> GetWeeklyPeriodicity(
        GetWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetWeeklyPeriodicityAsync(i, context.CancellationToken))
            .IfSuccessAsync(i => converter.Convert<GetWeeklyPeriodicityReply>(i))
            .HandleAsync(serializer);
    }

    public override Task<GetAnnuallyPeriodicityReply> GetAnnuallyPeriodicity(
        GetAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetAnnuallyPeriodicityAsync(i, context.CancellationToken))
            .IfSuccessAsync(p => converter.Convert<GetAnnuallyPeriodicityReply>(p))
            .HandleAsync(serializer);
    }

    public override Task<GetPeriodicityToDoItemSettingsReply> GetPeriodicityToDoItemSettings(
        GetPeriodicityToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetPeriodicityToDoItemSettingsAsync(i, context.CancellationToken))
            .IfSuccessAsync(s => converter.Convert<GetPeriodicityToDoItemSettingsReply>(s))
            .HandleAsync(serializer, p => p);
    }

    public override Task<GetValueToDoItemSettingsReply> GetValueToDoItemSettings(
        GetValueToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetValueToDoItemSettingsAsync(i, context.CancellationToken))
            .IfSuccessAsync(s => converter.Convert<GetValueToDoItemSettingsReply>(s))
            .HandleAsync(serializer);
    }

    public override Task<GetPlannedToDoItemSettingsReply> GetPlannedToDoItemSettings(
        GetPlannedToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetPlannedToDoItemSettingsAsync(i, context.CancellationToken))
            .IfSuccessAsync(s => converter.Convert<GetPlannedToDoItemSettingsReply>(s))
            .HandleAsync(serializer);
    }

    public override Task<GetParentsReply> GetParents(GetParentsRequest request, ServerCallContext context)
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetParentsAsync(i, context.CancellationToken))
            .IfSuccessAsync(p => converter.Convert<ToDoShortItemGrpc[]>(p.ToArray()))
            .HandleAsync(
                serializer,
                parents =>
                {
                    var reply = new GetParentsReply();
                    reply.Parents.AddRange(parents);

                    return reply;
                }
            );
    }

    public override Task<GetRootToDoItemIdsReply> GetRootToDoItemIds(
        GetRootToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetRootToDoItemIdsAsync(context.CancellationToken)
            .IfSuccessAsync(ids => converter.Convert<ByteString[]>(ids.ToArray()))
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetRootToDoItemIdsReply();
                    reply.Ids.AddRange(ids);

                    return reply;
                }
            );
    }

    public override Task<GetLeafToDoItemIdsReply> GetLeafToDoItemIds(
        GetLeafToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetLeafToDoItemIdsAsync(i, context.CancellationToken))
            .IfSuccessAsync(ids => converter.Convert<ByteString[]>(ids.ToArray()))
            .HandleAsync(
                serializer,
                s =>
                {
                    var reply = new GetLeafToDoItemIdsReply();
                    reply.Ids.AddRange(s);

                    return reply;
                }
            );
    }

    public override Task<GetFavoriteToDoItemIdsRequestReply> GetFavoriteToDoItemIds(
        GetFavoriteToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetFavoriteToDoItemIdsAsync(context.CancellationToken)
            .IfSuccessAsync(ids => converter.Convert<ByteString[]>(ids.ToArray()))
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetFavoriteToDoItemIdsRequestReply();
                    reply.Ids.AddRange(ids);

                    return reply;
                }
            );
    }

    public override Task<GetChildrenToDoItemIdsReply> GetChildrenToDoItemIds(
        GetChildrenToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(i => toDoService.GetChildrenToDoItemIdsAsync(i, context.CancellationToken))
            .IfSuccessAsync(ids => converter.Convert<ByteString[]>(ids.ToArray()))
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetChildrenToDoItemIdsReply();
                    reply.Ids.AddRange(ids);

                    return reply;
                }
            );
    }

    public override Task<SearchToDoItemIdsReply> SearchToDoItemIds(
        SearchToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService.SearchToDoItemIdsAsync(request.SearchText, context.CancellationToken)
            .IfSuccessAsync(ids => converter.Convert<ByteString[]>(ids.ToArray()))
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new SearchToDoItemIdsReply();
                    reply.Ids.AddRange(ids);

                    return reply;
                }
            );
    }

    public override Task<GetToDoItemReply> GetToDoItem(GetToDoItemRequest request, ServerCallContext context)
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(id => toDoService.GetToDoItemAsync(id, context.CancellationToken))
            .IfSuccessAsync(toDoItem => converter.Convert<GetToDoItemReply>(toDoItem))
            .HandleAsync(serializer);
    }

    public override Task<GetCurrentActiveToDoItemReply> GetCurrentActiveToDoItem(
        GetCurrentActiveToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService.GetCurrentActiveToDoItemAsync(context.CancellationToken)
            .IfSuccessAsync(toDoItem => converter.Convert<ActiveToDoItemGrpc>(toDoItem))
            .HandleAsync(
                serializer,
                re => new GetCurrentActiveToDoItemReply
                {
                    Item = re
                }
            );
    }

    public override Task<UpdateToDoItemLinkReply> UpdateToDoItemLink(
        UpdateToDoItemLinkRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                converter.Convert<Uri>(request.Link),
                (id, link) => toDoService.UpdateToDoItemLinkAsync(id, link, context.CancellationToken)
            )
            .HandleAsync<UpdateToDoItemLinkReply>(serializer);
    }

    public override Task<AddRootToDoItemReply> AddRootToDoItem(
        AddRootToDoItemRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<AddRootToDoItemOptions>(request)
            .IfSuccessAsync(
                options => toDoService.AddRootToDoItemAsync(
                    options,
                    context.CancellationToken
                )
            )
            .IfSuccessAsync(id => converter.Convert<ByteString>(id))
            .HandleAsync(
                serializer,
                id =>
                    new AddRootToDoItemReply
                    {
                        Id = id
                    }
            );
    }

    public override Task<AddToDoItemReply> AddToDoItem(AddToDoItemRequest request, ServerCallContext context)
    {
        return converter.Convert<AddToDoItemOptions>(request)
            .IfSuccessAsync(
                options => toDoService.AddToDoItemAsync(
                    options,
                    context.CancellationToken
                )
            )
            .IfSuccessAsync(id => converter.Convert<ByteString>(id))
            .HandleAsync(
                serializer,
                id => new AddToDoItemReply
                {
                    Id = id,
                }
            );
    }

    public override Task<DeleteToDoItemReply> DeleteToDoItem(
        DeleteToDoItemRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(id => toDoService.DeleteToDoItemAsync(id, context.CancellationToken))
            .HandleAsync<DeleteToDoItemReply>(serializer);
    }

    public override Task<UpdateToDoItemTypeOfPeriodicityReply> UpdateToDoItemTypeOfPeriodicity(
        UpdateToDoItemTypeOfPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemTypeOfPeriodicityAsync(
                    id,
                    (TypeOfPeriodicity)request.Type,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemTypeOfPeriodicityReply>(serializer);
        ;
    }

    public override Task<UpdateToDoItemDueDateReply> UpdateToDoItemDueDate(
        UpdateToDoItemDueDateRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                converter.Convert<DateOnly>(request.DueDate),
                (id, dueDate) => toDoService.UpdateToDoItemDueDateAsync(
                    id,
                    dueDate,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemDueDateReply>(serializer);
    }

    public override Task<UpdateToDoItemCompleteStatusReply> UpdateToDoItemCompleteStatus(
        UpdateToDoItemCompleteStatusRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemCompleteStatusAsync(id, request.IsCompleted, context.CancellationToken)
            )
            .HandleAsync<UpdateToDoItemCompleteStatusReply>(serializer);
    }

    public override Task<UpdateToDoItemNameReply> UpdateToDoItemName(
        UpdateToDoItemNameRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemNameAsync(
                    id,
                    request.Name,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemNameReply>(serializer);
    }

    public override Task<UpdateToDoItemOrderIndexReply> UpdateToDoItemOrderIndex(
        UpdateToDoItemOrderIndexRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<UpdateOrderIndexToDoItemOptions>(request)
            .IfSuccessAsync(options => toDoService.UpdateToDoItemOrderIndexAsync(options, context.CancellationToken))
            .HandleAsync<UpdateToDoItemOrderIndexReply>(serializer);
    }

    public override Task<UpdateToDoItemDescriptionReply> UpdateToDoItemDescription(
        UpdateToDoItemDescriptionRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemDescriptionAsync(
                    id,
                    request.Description,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemDescriptionReply>(serializer);
    }

    public override Task<UpdateToDoItemTypeReply> UpdateToDoItemType(
        UpdateToDoItemTypeRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemTypeAsync(
                    id,
                    (ToDoItemType)request.Type,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemTypeReply>(serializer);
    }

    public override Task<AddFavoriteToDoItemReply> AddFavoriteToDoItem(
        AddFavoriteToDoItemRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(id => toDoService.AddFavoriteToDoItemAsync(id, context.CancellationToken))
            .HandleAsync<AddFavoriteToDoItemReply>(serializer);
    }

    public override Task<RemoveFavoriteToDoItemReply> RemoveFavoriteToDoItem(
        RemoveFavoriteToDoItemRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(id => toDoService.RemoveFavoriteToDoItemAsync(id, context.CancellationToken))
            .HandleAsync<RemoveFavoriteToDoItemReply>(serializer);
    }

    public override Task<UpdateToDoItemAnnuallyPeriodicityReply> UpdateToDoItemAnnuallyPeriodicity(
        UpdateToDoItemAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                converter.Convert<AnnuallyPeriodicity>(request.Periodicity),
                (id, periodicity) => toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
                    id,
                    periodicity,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemAnnuallyPeriodicityReply>(serializer);
    }

    public override Task<UpdateToDoItemMonthlyPeriodicityReply> UpdateToDoItemMonthlyPeriodicity(
        UpdateToDoItemMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                converter.Convert<MonthlyPeriodicity>(request.Periodicity),
                (id, periodicity) => toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
                    id,
                    periodicity,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemMonthlyPeriodicityReply>(serializer);
    }

    public override Task<UpdateToDoItemWeeklyPeriodicityReply> UpdateToDoItemWeeklyPeriodicity(
        UpdateToDoItemWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                converter.Convert<WeeklyPeriodicity>(request.Periodicity),
                (id, periodicity) => toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
                    id,
                    periodicity,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemWeeklyPeriodicityReply>(serializer);
    }

    public override Task<GetToDoSelectorItemsReply> GetToDoSelectorItems(
        GetToDoSelectorItemsRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid[]>(request.IgnoreIds)
            .IfSuccessAsync(
                ignoreIds => toDoService.GetToDoSelectorItemsAsync(
                    ignoreIds,
                    context.CancellationToken
                )
            )
            .IfSuccessAsync(toDoSelectorItems => converter.Convert<ToDoSelectorItemGrpc[]>(toDoSelectorItems.ToArray()))
            .HandleAsync(
                serializer,
                toDoSelectorItems =>
                {
                    if (toDoSelectorItems == null)
                        throw new ArgumentNullException(nameof(toDoSelectorItems));
                    var reply = new GetToDoSelectorItemsReply();
                    reply.Items.AddRange(toDoSelectorItems);

                    return reply;
                }
            );
    }

    public override Task<UpdateToDoItemParentReply> UpdateToDoItemParent(
        UpdateToDoItemParentRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                converter.Convert<Guid>(request.ParentId),
                (id, pi) => toDoService.UpdateToDoItemParentAsync(id, pi, context.CancellationToken)
            )
            .HandleAsync<UpdateToDoItemParentReply>(serializer);
    }

    public override Task<ToDoItemToRootReply> ToDoItemToRoot(ToDoItemToRootRequest request, ServerCallContext context)
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(id => toDoService.ToDoItemToRootAsync(id, context.CancellationToken))
            .HandleAsync<ToDoItemToRootReply>(serializer);
    }

    public override Task<ToDoItemToStringReply> ToDoItemToString(
        ToDoItemToStringRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<ToDoItemToStringOptions>(request)
            .IfSuccessAsync(options => toDoService.ToDoItemToStringAsync(options, context.CancellationToken))
            .HandleAsync(
                serializer,
                value => new ToDoItemToStringReply
                {
                    Value = value,
                }
            );
    }

    public override Task<UpdateToDoItemDaysOffsetReply> UpdateToDoItemDaysOffset(
        UpdateToDoItemDaysOffsetRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemDaysOffsetAsync(id, (ushort)request.Days, context.CancellationToken)
            )
            .HandleAsync<UpdateToDoItemDaysOffsetReply>(serializer);
    }

    public override Task<UpdateToDoItemMonthsOffsetReply> UpdateToDoItemMonthsOffset(
        UpdateToDoItemMonthsOffsetRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemMonthsOffsetAsync(
                    id,
                    (ushort)request.Months,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemMonthsOffsetReply>(serializer);
    }

    public override Task<UpdateToDoItemWeeksOffsetReply> UpdateToDoItemWeeksOffset(
        UpdateToDoItemWeeksOffsetRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemWeeksOffsetAsync(
                    id,
                    (ushort)request.Weeks,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemWeeksOffsetReply>(serializer);
    }

    public override Task<UpdateToDoItemYearsOffsetReply> UpdateToDoItemYearsOffset(
        UpdateToDoItemYearsOffsetRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemYearsOffsetAsync(
                    id,
                    (ushort)request.Years,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemYearsOffsetReply>(serializer);
    }

    public override Task<UpdateToDoItemChildrenTypeReply> UpdateToDoItemChildrenType(
        UpdateToDoItemChildrenTypeRequest request,
        ServerCallContext context
    )
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(
                id => toDoService.UpdateToDoItemChildrenTypeAsync(
                    id,
                    (ToDoItemChildrenType)request.Type,
                    context.CancellationToken
                )
            )
            .HandleAsync<UpdateToDoItemChildrenTypeReply>(serializer);
    }

    public override Task<GetSiblingsReply> GetSiblings(GetSiblingsRequest request, ServerCallContext context)
    {
        return converter.Convert<Guid>(request.Id)
            .IfSuccessAsync(id => toDoService.GetSiblingsAsync(id, context.CancellationToken))
            .IfSuccessAsync(items => converter.Convert<ToDoShortItemGrpc[]>(items.ToArray()))
            .HandleAsync(
                serializer,
                items =>
                {
                    var reply = new GetSiblingsReply();
                    reply.Items.AddRange(items);

                    return reply;
                }
            );
    }
}