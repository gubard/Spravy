using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Mapper.Mappers;
using Spravy.ToDo.Protos;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcRouterToDoService : ToDoService.ToDoServiceBase
{
    private readonly ISerializer serializer;
    private readonly IToDoService toDoService;

    public GrpcRouterToDoService(IToDoService toDoService, ISerializer serializer)
    {
        this.toDoService = toDoService;
        this.serializer = serializer;
    }

    public override Task<UpdateToDoItemIsRequiredCompleteInDueDateReply> UpdateToDoItemIsRequiredCompleteInDueDate(
        UpdateToDoItemIsRequiredCompleteInDueDateRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                request.Id.ToGuid(),
                request.IsRequiredCompleteInDueDate,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemIsRequiredCompleteInDueDateReply>(
                serializer,
                context.CancellationToken
            );
    }

    public override Task<GetTodayToDoItemsReply> GetTodayToDoItems(
        GetTodayToDoItemsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetTodayToDoItemsAsync(context.CancellationToken)
            .HandleAsync(
                serializer,
                ids =>
                {
                    var result = new GetTodayToDoItemsReply();
                    result.Ids.AddRange(ids.ToByteString().ToArray());

                    return result;
                },
                context.CancellationToken
            );
    }

    public override Task<ResetToDoItemReply> ResetToDoItem(
        ResetToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .ResetToDoItemAsync(request.ToResetToDoItemOptions(), context.CancellationToken)
            .HandleAsync<ResetToDoItemReply>(serializer, context.CancellationToken);
    }

    public override Task<GetPeriodicityOffsetToDoItemSettingsReply> GetPeriodicityOffsetToDoItemSettings(
        GetPeriodicityOffsetToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetPeriodicityOffsetToDoItemSettingsAsync(
                request.Id.ToGuid(),
                context.CancellationToken
            )
            .HandleAsync(
                serializer,
                settings =>
                {
                    var reply = settings.ToGetPeriodicityOffsetToDoItemSettingsReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetValueToDoItemSettingsReply> GetValueToDoItemSettings(
        GetValueToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetValueToDoItemSettingsAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                settings =>
                {
                    var reply = settings.ToGetValueToDoItemSettingsReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetRootToDoItemIdsReply> GetRootToDoItemIds(
        GetRootToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetRootToDoItemIdsAsync(context.CancellationToken)
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetRootToDoItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetPlannedToDoItemSettingsReply> GetPlannedToDoItemSettings(
        GetPlannedToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetPlannedToDoItemSettingsAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                settings =>
                {
                    var reply = settings.ToGetPlannedToDoItemSettingsReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetPeriodicityToDoItemSettingsReply> GetPeriodicityToDoItemSettings(
        GetPeriodicityToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetPeriodicityToDoItemSettingsAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                settings =>
                {
                    var reply = settings.ToGetPeriodicityToDoItemSettingsReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetLeafToDoItemIdsReply> GetLeafToDoItemIds(
        GetLeafToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetLeafToDoItemIdsAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetLeafToDoItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetFavoriteToDoItemIdsRequestReply> GetFavoriteToDoItemIds(
        GetFavoriteToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetFavoriteToDoItemIdsAsync(context.CancellationToken)
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetFavoriteToDoItemIdsRequestReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetCurrentActiveToDoItemReply> GetCurrentActiveToDoItem(
        GetCurrentActiveToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetCurrentActiveToDoItemAsync(context.CancellationToken)
            .HandleAsync(
                serializer,
                item =>
                {
                    var reply = new GetCurrentActiveToDoItemReply();
                    reply.Item = item.ToActiveToDoItemGrpc();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetChildrenToDoItemIdsReply> GetChildrenToDoItemIds(
        GetChildrenToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetChildrenToDoItemIdsAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new GetChildrenToDoItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<UpdateToDoItemLinkReply> UpdateToDoItemLink(
        UpdateToDoItemLinkRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemLinkAsync(
                request.Id.ToGuid(),
                request.Link.ToOptionUri(),
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemLinkReply>(serializer, context.CancellationToken);
    }

    public override Task<SearchToDoItemIdsReply> SearchToDoItemIds(
        SearchToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .SearchToDoItemIdsAsync(request.SearchText, context.CancellationToken)
            .HandleAsync(
                serializer,
                ids =>
                {
                    var reply = new SearchToDoItemIdsReply();
                    reply.Ids.AddRange(ids.ToByteString().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<RandomizeChildrenOrderIndexReply> RandomizeChildrenOrderIndex(
        RandomizeChildrenOrderIndexRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .RandomizeChildrenOrderIndexAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync<RandomizeChildrenOrderIndexReply>(serializer, context.CancellationToken);
    }

    public override async Task GetToDoItems(
        GetToDoItemsRequest request,
        IServerStreamWriter<GetToDoItemsReply> responseStream,
        ServerCallContext context
    )
    {
        var ids = request.Ids.ToGuid();

        await foreach (
            var item in toDoService.GetToDoItemsAsync(
                ids,
                request.ChunkSize,
                context.CancellationToken
            )
        )
        {
            var reply = new GetToDoItemsReply();
            reply.Items.AddRange(item.ThrowIfError().ToToDoItemGrpc().ToArray());
            await responseStream.WriteAsync(reply);
        }
    }

    public override Task<GetToDoItemReply> GetToDoItem(
        GetToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetToDoItemAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                item =>
                {
                    var reply = item.ToGetToDoItemReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetWeeklyPeriodicityReply> GetWeeklyPeriodicity(
        GetWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetWeeklyPeriodicityAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                periodicity =>
                {
                    var reply = periodicity.ToGetWeeklyPeriodicityReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetMonthlyPeriodicityReply> GetMonthlyPeriodicity(
        GetMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetMonthlyPeriodicityAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                periodicity =>
                {
                    var reply = periodicity.ToGetMonthlyPeriodicityReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetAnnuallyPeriodicityReply> GetAnnuallyPeriodicity(
        GetAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetAnnuallyPeriodicityAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                periodicity =>
                {
                    var reply = periodicity.ToGetAnnuallyPeriodicityReply();

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetSiblingsReply> GetSiblings(
        GetSiblingsRequest request,
        ServerCallContext context
    )
    {
        var id = request.Id.ToGuid();

        return toDoService
            .GetSiblingsAsync(id, context.CancellationToken)
            .HandleAsync(
                serializer,
                items =>
                {
                    var reply = new GetSiblingsReply();
                    reply.Items.AddRange(items.ToToDoShortItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<GetParentsReply> GetParents(
        GetParentsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetParentsAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                parents =>
                {
                    var reply = new GetParentsReply();
                    reply.Parents.AddRange(parents.ToToDoShortItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<AddToDoItemReply> AddToDoItem(
        AddToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .AddToDoItemAsync(request.ToAddToDoItemOptions(), context.CancellationToken)
            .HandleAsync(
                serializer,
                id =>
                {
                    var reply = new AddToDoItemReply { Id = id.ToByteString() };

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<DeleteToDoItemReply> DeleteToDoItem(
        DeleteToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .DeleteToDoItemAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync<DeleteToDoItemReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemTypeOfPeriodicityReply> UpdateToDoItemTypeOfPeriodicity(
        UpdateToDoItemTypeOfPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemTypeOfPeriodicityAsync(
                request.Id.ToGuid(),
                (TypeOfPeriodicity)request.Type,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemTypeOfPeriodicityReply>(
                serializer,
                context.CancellationToken
            );
    }

    public override Task<UpdateToDoItemDueDateReply> UpdateToDoItemDueDate(
        UpdateToDoItemDueDateRequest request,
        ServerCallContext context
    )
    {
        var dueDate = request.DueDate.ToDateOnly();

        return toDoService
            .UpdateToDoItemDueDateAsync(request.Id.ToGuid(), dueDate, context.CancellationToken)
            .HandleAsync<UpdateToDoItemDueDateReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemCompleteStatusReply> UpdateToDoItemCompleteStatus(
        UpdateToDoItemCompleteStatusRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemCompleteStatusAsync(
                request.Id.ToGuid(),
                request.IsCompleted,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemCompleteStatusReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemNameReply> UpdateToDoItemName(
        UpdateToDoItemNameRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemNameAsync(request.Id.ToGuid(), request.Name, context.CancellationToken)
            .HandleAsync<UpdateToDoItemNameReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemOrderIndexReply> UpdateToDoItemOrderIndex(
        UpdateToDoItemOrderIndexRequest request,
        ServerCallContext context
    )
    {
        var options = request.ToUpdateOrderIndexToDoItemOptions();

        return toDoService
            .UpdateToDoItemOrderIndexAsync(options, context.CancellationToken)
            .HandleAsync<UpdateToDoItemOrderIndexReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemDescriptionReply> UpdateToDoItemDescription(
        UpdateToDoItemDescriptionRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemDescriptionAsync(
                request.Id.ToGuid(),
                request.Description,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemDescriptionReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemTypeReply> UpdateToDoItemType(
        UpdateToDoItemTypeRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemTypeAsync(
                request.Id.ToGuid(),
                (ToDoItemType)request.Type,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemTypeReply>(serializer, context.CancellationToken);
    }

    public override Task<AddFavoriteToDoItemReply> AddFavoriteToDoItem(
        AddFavoriteToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .AddFavoriteToDoItemAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync<AddFavoriteToDoItemReply>(serializer, context.CancellationToken);
    }

    public override Task<RemoveFavoriteToDoItemReply> RemoveFavoriteToDoItem(
        RemoveFavoriteToDoItemRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .RemoveFavoriteToDoItemAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync<RemoveFavoriteToDoItemReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemAnnuallyPeriodicityReply> UpdateToDoItemAnnuallyPeriodicity(
        UpdateToDoItemAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemAnnuallyPeriodicityAsync(
                request.Id.ToGuid(),
                request.Periodicity.ToAnnuallyPeriodicity(),
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemAnnuallyPeriodicityReply>(
                serializer,
                context.CancellationToken
            );
    }

    public override Task<UpdateToDoItemMonthlyPeriodicityReply> UpdateToDoItemMonthlyPeriodicity(
        UpdateToDoItemMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemMonthlyPeriodicityAsync(
                request.Id.ToGuid(),
                request.Periodicity.ToMonthlyPeriodicity(),
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemMonthlyPeriodicityReply>(
                serializer,
                context.CancellationToken
            );
    }

    public override Task<UpdateToDoItemWeeklyPeriodicityReply> UpdateToDoItemWeeklyPeriodicity(
        UpdateToDoItemWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemWeeklyPeriodicityAsync(
                request.Id.ToGuid(),
                request.Periodicity.ToWeeklyPeriodicity(),
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemWeeklyPeriodicityReply>(
                serializer,
                context.CancellationToken
            );
    }

    public override Task<GetToDoSelectorItemsReply> GetToDoSelectorItems(
        GetToDoSelectorItemsRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .GetToDoSelectorItemsAsync(request.IgnoreIds.ToGuid(), context.CancellationToken)
            .HandleAsync(
                serializer,
                items =>
                {
                    var reply = new GetToDoSelectorItemsReply();
                    reply.Items.AddRange(items.ToToDoSelectorItemGrpc().ToArray());

                    return reply;
                },
                context.CancellationToken
            );
    }

    public override Task<UpdateToDoItemParentReply> UpdateToDoItemParent(
        UpdateToDoItemParentRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemParentAsync(
                request.Id.ToGuid(),
                request.ParentId.ToGuid(),
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemParentReply>(serializer, context.CancellationToken);
    }

    public override Task<ToDoItemToRootReply> ToDoItemToRoot(
        ToDoItemToRootRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .ToDoItemToRootAsync(request.Id.ToGuid(), context.CancellationToken)
            .HandleAsync<ToDoItemToRootReply>(serializer, context.CancellationToken);
    }

    public override Task<ToDoItemToStringReply> ToDoItemToString(
        ToDoItemToStringRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .ToDoItemToStringAsync(request.ToToDoItemToStringOptions(), context.CancellationToken)
            .HandleAsync(
                serializer,
                value => new ToDoItemToStringReply { Value = value },
                context.CancellationToken
            );
    }

    public override Task<UpdateToDoItemDaysOffsetReply> UpdateToDoItemDaysOffset(
        UpdateToDoItemDaysOffsetRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemDaysOffsetAsync(
                request.Id.ToGuid(),
                (ushort)request.Days,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemDaysOffsetReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemMonthsOffsetReply> UpdateToDoItemMonthsOffset(
        UpdateToDoItemMonthsOffsetRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemMonthsOffsetAsync(
                request.Id.ToGuid(),
                (ushort)request.Months,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemMonthsOffsetReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemWeeksOffsetReply> UpdateToDoItemWeeksOffset(
        UpdateToDoItemWeeksOffsetRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemWeeksOffsetAsync(
                request.Id.ToGuid(),
                (ushort)request.Weeks,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemWeeksOffsetReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemYearsOffsetReply> UpdateToDoItemYearsOffset(
        UpdateToDoItemYearsOffsetRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemYearsOffsetAsync(
                request.Id.ToGuid(),
                (ushort)request.Years,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemYearsOffsetReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateToDoItemChildrenTypeReply> UpdateToDoItemChildrenType(
        UpdateToDoItemChildrenTypeRequest request,
        ServerCallContext context
    )
    {
        return toDoService
            .UpdateToDoItemChildrenTypeAsync(
                request.Id.ToGuid(),
                (ToDoItemChildrenType)request.Type,
                context.CancellationToken
            )
            .HandleAsync<UpdateToDoItemChildrenTypeReply>(serializer, context.CancellationToken);
    }
}
