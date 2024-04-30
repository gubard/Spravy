using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcToDoService : ToDoService.ToDoServiceBase
{
    private readonly IMapper mapper;
    private readonly ISerializer serializer;
    private readonly IToDoService toDoService;

    public GrpcToDoService(IToDoService toDoService, IMapper mapper, ISerializer serializer)
    {
        this.toDoService = toDoService;
        this.mapper = mapper;
        this.serializer = serializer;
    }

    public override async Task<UpdateToDoItemIsRequiredCompleteInDueDateReply>
        UpdateToDoItemIsRequiredCompleteInDueDate(
            UpdateToDoItemIsRequiredCompleteInDueDateRequest request,
            ServerCallContext context
        )
    {
        await toDoService.UpdateToDoItemIsRequiredCompleteInDueDateAsync(mapper.Map<Guid>(request.Id),
            request.IsRequiredCompleteInDueDate, context.CancellationToken);

        return new();
    }

    public override async Task<GetTodayToDoItemsReply> GetTodayToDoItems(
        GetTodayToDoItemsRequest request,
        ServerCallContext context
    )
    {
        var ids = await toDoService.GetTodayToDoItemsAsync(context.CancellationToken);
        var result = new GetTodayToDoItemsReply();
        result.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));

        return result;
    }

    public override async Task<ResetToDoItemReply> ResetToDoItem(
        ResetToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.ResetToDoItemAsync(mapper.Map<ResetToDoItemOptions>(request), context.CancellationToken);

        return new();
    }

    public override async Task<GetPeriodicityOffsetToDoItemSettingsReply> GetPeriodicityOffsetToDoItemSettings(
        GetPeriodicityOffsetToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        var settings = await toDoService.GetPeriodicityOffsetToDoItemSettingsAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetPeriodicityOffsetToDoItemSettingsReply>(settings);

        return reply;
    }

    public override async Task<GetValueToDoItemSettingsReply> GetValueToDoItemSettings(
        GetValueToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        var settings = await toDoService.GetValueToDoItemSettingsAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetValueToDoItemSettingsReply>(settings);

        return reply;
    }

    public override async Task<GetRootToDoItemIdsReply> GetRootToDoItemIds(
        GetRootToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        var ids = await toDoService.GetRootToDoItemIdsAsync(context.CancellationToken);
        var reply = new GetRootToDoItemIdsReply();
        reply.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));

        return reply;
    }

    public override async Task<GetPlannedToDoItemSettingsReply> GetPlannedToDoItemSettings(
        GetPlannedToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        var settings = await toDoService.GetPlannedToDoItemSettingsAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetPlannedToDoItemSettingsReply>(settings);

        return reply;
    }

    public override async Task<GetPeriodicityToDoItemSettingsReply> GetPeriodicityToDoItemSettings(
        GetPeriodicityToDoItemSettingsRequest request,
        ServerCallContext context
    )
    {
        var settings = await toDoService.GetPeriodicityToDoItemSettingsAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetPeriodicityToDoItemSettingsReply>(settings);

        return reply;
    }

    public override async Task<GetLeafToDoItemIdsReply> GetLeafToDoItemIds(
        GetLeafToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        var ids = await toDoService.GetLeafToDoItemIdsAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);
        var reply = new GetLeafToDoItemIdsReply();
        reply.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));

        return reply;
    }

    public override async Task<GetFavoriteToDoItemIdsRequestReply> GetFavoriteToDoItemIds(
        GetFavoriteToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        var ids = await toDoService.GetFavoriteToDoItemIdsAsync(context.CancellationToken);
        var reply = new GetFavoriteToDoItemIdsRequestReply();
        reply.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));

        return reply;
    }

    public override async Task<GetCurrentActiveToDoItemReply> GetCurrentActiveToDoItem(
        GetCurrentActiveToDoItemRequest request,
        ServerCallContext context
    )
    {
        var reply = new GetCurrentActiveToDoItemReply();
        var item = await toDoService.GetCurrentActiveToDoItemAsync(context.CancellationToken);
        reply.Item = mapper.Map<ActiveToDoItemGrpc>(item);

        return reply;
    }

    public override async Task<GetChildrenToDoItemIdsReply> GetChildrenToDoItemIds(
        GetChildrenToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        var ids = await toDoService.GetChildrenToDoItemIdsAsync(mapper.Map<Guid>(request.Id),
            context.CancellationToken);

        var reply = new GetChildrenToDoItemIdsReply();
        reply.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));

        return reply;
    }

    public override async Task<UpdateToDoItemLinkReply> UpdateToDoItemLink(
        UpdateToDoItemLinkRequest request,
        ServerCallContext context
    )
    {
        var reply = new UpdateToDoItemLinkReply();

        await toDoService.UpdateToDoItemLinkAsync(mapper.Map<Guid>(request.Id), mapper.Map<Uri>(request.Link),
            context.CancellationToken);

        return reply;
    }

    public override async Task<SearchToDoItemIdsReply> SearchToDoItemIds(
        SearchToDoItemIdsRequest request,
        ServerCallContext context
    )
    {
        var ids = await toDoService.SearchToDoItemIdsAsync(request.SearchText, context.CancellationToken);
        var reply = new SearchToDoItemIdsReply();
        reply.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));

        return reply;
    }

    public override async Task<RandomizeChildrenOrderIndexReply> RandomizeChildrenOrderIndex(
        RandomizeChildrenOrderIndexRequest request,
        ServerCallContext context
    )
    {
        await toDoService.RandomizeChildrenOrderIndexAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return DefaultObject<RandomizeChildrenOrderIndexReply>.Default;
    }

    public override async Task GetToDoItems(
        GetToDoItemsRequest request,
        IServerStreamWriter<GetToDoItemsReply> responseStream,
        ServerCallContext context
    )
    {
        var ids = mapper.Map<Guid[]>(request.Ids);

        await foreach (var item in toDoService.GetToDoItemsAsync(ids, request.ChunkSize, context.CancellationToken))
        {
            var reply = new GetToDoItemsReply();
            reply.Items.AddRange(mapper.Map<IEnumerable<ToDoItemGrpc>>(item));
            await responseStream.WriteAsync(reply);
        }
    }

    public override async Task<GetToDoItemReply> GetToDoItem(GetToDoItemRequest request, ServerCallContext context)
    {
        var item = await toDoService.GetToDoItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);
        var reply = mapper.Map<GetToDoItemReply>(item);

        return reply;
    }

    public override async Task<GetWeeklyPeriodicityReply> GetWeeklyPeriodicity(
        GetWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        var periodicity = await toDoService.GetWeeklyPeriodicityAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetWeeklyPeriodicityReply>(periodicity);

        return reply;
    }

    public override async Task<GetMonthlyPeriodicityReply> GetMonthlyPeriodicity(
        GetMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        var periodicity = await toDoService.GetMonthlyPeriodicityAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetMonthlyPeriodicityReply>(periodicity);

        return reply;
    }

    public override async Task<GetAnnuallyPeriodicityReply> GetAnnuallyPeriodicity(
        GetAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        var periodicity = await toDoService.GetAnnuallyPeriodicityAsync(
            mapper.Map<Guid>(request.Id), context.CancellationToken);

        var reply = mapper.Map<GetAnnuallyPeriodicityReply>(periodicity);

        return reply;
    }

    public override async Task<GetSiblingsReply> GetSiblings(GetSiblingsRequest request, ServerCallContext context)
    {
        var id = mapper.Map<Guid>(request.Id);
        var items = await toDoService.GetSiblingsAsync(id, context.CancellationToken);
        var reply = new GetSiblingsReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoShortItemGrpc>>(items));

        return reply;
    }

    public override async Task<GetParentsReply> GetParents(GetParentsRequest request, ServerCallContext context)
    {
        var parents = await toDoService.GetParentsAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);
        var reply = new GetParentsReply();
        reply.Parents.AddRange(mapper.Map<IEnumerable<ToDoShortItemGrpc>>(parents));

        return reply;
    }

    public override async Task<AddRootToDoItemReply> AddRootToDoItem(
        AddRootToDoItemRequest request,
        ServerCallContext context
    )
    {
        var id = await toDoService.AddRootToDoItemAsync(mapper.Map<AddRootToDoItemOptions>(request),
            context.CancellationToken);

        var reply = new AddRootToDoItemReply
        {
            Id = mapper.Map<ByteString>(id),
        };

        return reply;
    }

    public override async Task<AddToDoItemReply> AddToDoItem(AddToDoItemRequest request, ServerCallContext context)
    {
        var id = await toDoService.AddToDoItemAsync(mapper.Map<AddToDoItemOptions>(request), context.CancellationToken);

        var reply = new AddToDoItemReply
        {
            Id = mapper.Map<ByteString>(id),
        };

        return reply;
    }

    public override async Task<DeleteToDoItemReply> DeleteToDoItem(
        DeleteToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.DeleteToDoItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemTypeOfPeriodicityReply> UpdateToDoItemTypeOfPeriodicity(
        UpdateToDoItemTypeOfPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemTypeOfPeriodicityAsync(mapper.Map<Guid>(request.Id),
            (TypeOfPeriodicity)request.Type, context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemDueDateReply> UpdateToDoItemDueDate(
        UpdateToDoItemDueDateRequest request,
        ServerCallContext context
    )
    {
        var dueDate = mapper.Map<DateOnly>(request.DueDate);
        await toDoService.UpdateToDoItemDueDateAsync(mapper.Map<Guid>(request.Id), dueDate, context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemCompleteStatusReply> UpdateToDoItemCompleteStatus(
        UpdateToDoItemCompleteStatusRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemCompleteStatusAsync(mapper.Map<Guid>(request.Id), request.IsCompleted,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemNameReply> UpdateToDoItemName(
        UpdateToDoItemNameRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemNameAsync(mapper.Map<Guid>(request.Id), request.Name,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemOrderIndexReply> UpdateToDoItemOrderIndex(
        UpdateToDoItemOrderIndexRequest request,
        ServerCallContext context
    )
    {
        var options = mapper.Map<UpdateOrderIndexToDoItemOptions>(request);
        await toDoService.UpdateToDoItemOrderIndexAsync(options, context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemDescriptionReply> UpdateToDoItemDescription(
        UpdateToDoItemDescriptionRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemDescriptionAsync(mapper.Map<Guid>(request.Id), request.Description,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemTypeReply> UpdateToDoItemType(
        UpdateToDoItemTypeRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemTypeAsync(mapper.Map<Guid>(request.Id), (ToDoItemType)request.Type,
            context.CancellationToken);

        return new();
    }

    public override async Task<AddFavoriteToDoItemReply> AddFavoriteToDoItem(
        AddFavoriteToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.AddFavoriteToDoItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new();
    }

    public override async Task<RemoveFavoriteToDoItemReply> RemoveFavoriteToDoItem(
        RemoveFavoriteToDoItemRequest request,
        ServerCallContext context
    )
    {
        await toDoService.RemoveFavoriteToDoItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemAnnuallyPeriodicityReply> UpdateToDoItemAnnuallyPeriodicity(
        UpdateToDoItemAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(mapper.Map<Guid>(request.Id),
            mapper.Map<AnnuallyPeriodicity>(request.Periodicity), context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemMonthlyPeriodicityReply> UpdateToDoItemMonthlyPeriodicity(
        UpdateToDoItemMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemMonthlyPeriodicityAsync(mapper.Map<Guid>(request.Id),
            mapper.Map<MonthlyPeriodicity>(request.Periodicity), context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemWeeklyPeriodicityReply> UpdateToDoItemWeeklyPeriodicity(
        UpdateToDoItemWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemWeeklyPeriodicityAsync(mapper.Map<Guid>(request.Id),
            mapper.Map<WeeklyPeriodicity>(request.Periodicity), context.CancellationToken);

        return new();
    }

    public override async Task<GetToDoSelectorItemsReply> GetToDoSelectorItems(
        GetToDoSelectorItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.GetToDoSelectorItemsAsync(mapper.Map<Guid[]>(request.IgnoreIds),
            context.CancellationToken);

        var reply = new GetToDoSelectorItemsReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSelectorItemGrpc>>(items));

        return reply;
    }

    public override async Task<UpdateToDoItemParentReply> UpdateToDoItemParent(
        UpdateToDoItemParentRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemParentAsync(mapper.Map<Guid>(request.Id), mapper.Map<Guid>(request.ParentId),
            context.CancellationToken);

        return new();
    }

    public override async Task<ToDoItemToRootReply> ToDoItemToRoot(
        ToDoItemToRootRequest request,
        ServerCallContext context
    )
    {
        await toDoService.ToDoItemToRootAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new();
    }

    public override Task<ToDoItemToStringReply> ToDoItemToString(
        ToDoItemToStringRequest request,
        ServerCallContext context
    )
    {
        return toDoService
           .ToDoItemToStringAsync(mapper.Map<ToDoItemToStringOptions>(request), context.CancellationToken)
           .HandleAsync(serializer, value => new ToDoItemToStringReply
            {
                Value = value,
            });
    }

    public override async Task<UpdateToDoItemDaysOffsetReply> UpdateToDoItemDaysOffset(
        UpdateToDoItemDaysOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemDaysOffsetAsync(mapper.Map<Guid>(request.Id), (ushort)request.Days,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemMonthsOffsetReply> UpdateToDoItemMonthsOffset(
        UpdateToDoItemMonthsOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemMonthsOffsetAsync(mapper.Map<Guid>(request.Id), (ushort)request.Months,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemWeeksOffsetReply> UpdateToDoItemWeeksOffset(
        UpdateToDoItemWeeksOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemWeeksOffsetAsync(mapper.Map<Guid>(request.Id), (ushort)request.Weeks,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemYearsOffsetReply> UpdateToDoItemYearsOffset(
        UpdateToDoItemYearsOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemYearsOffsetAsync(mapper.Map<Guid>(request.Id), (ushort)request.Years,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateToDoItemChildrenTypeReply> UpdateToDoItemChildrenType(
        UpdateToDoItemChildrenTypeRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemChildrenTypeAsync(mapper.Map<Guid>(request.Id),
            (ToDoItemChildrenType)request.Type, context.CancellationToken);

        return new();
    }
}