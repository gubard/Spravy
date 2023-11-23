using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.Router.Service.Services;

[Authorize]
public class GrpcToDoService : ToDoService.ToDoServiceBase
{
    private readonly IToDoService toDoService;
    private readonly IMapper mapper;

    public GrpcToDoService(IToDoService toDoService, IMapper mapper)
    {
        this.toDoService = toDoService;
        this.mapper = mapper;
    }

    public override async Task<GetRootToDoSubItemsReply> GetRootToDoSubItems(
        GetRootToDoSubItemsRequest request,
        ServerCallContext context
    )
    {
        var reply = new GetRootToDoSubItemsReply();
        var items = await toDoService.GetRootToDoSubItemsAsync(context.CancellationToken);
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSubItemGrpc>>(items));

        return reply;
    }

    public override async Task<GetToDoItemReply2> GetToDoItem2(GetToDoItemRequest2 request, ServerCallContext context)
    {
        var item = await toDoService.GetToDoItem2Async(
            mapper.Map<Guid>(request.Id),
            context.CancellationToken
        );

        var reply = new GetToDoItemReply2
        {
            Item = mapper.Map<ToDoItemGrpc>(item),
        };

        return reply;
    }

    public override async Task<AddRootToDoItemReply> AddRootToDoItem(
        AddRootToDoItemRequest request,
        ServerCallContext context
    )
    {
        var id = await toDoService.AddRootToDoItemAsync(
            mapper.Map<AddRootToDoItemOptions>(request),
            context.CancellationToken
        );

        var reply = new AddRootToDoItemReply
        {
            Id = mapper.Map<ByteString>(id)
        };

        return reply;
    }

    public override async Task<AddToDoItemReply> AddToDoItem(AddToDoItemRequest request, ServerCallContext context)
    {
        var id = await toDoService.AddToDoItemAsync(
            mapper.Map<AddToDoItemOptions>(request),
            context.CancellationToken
        );

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
        await toDoService.UpdateToDoItemTypeOfPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            (TypeOfPeriodicity)request.Type,
            context.CancellationToken
        );

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
        await toDoService.UpdateToDoItemCompleteStatusAsync(
            mapper.Map<Guid>(request.Id),
            request.IsCompleted,
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemNameReply> UpdateToDoItemName(
        UpdateToDoItemNameRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemNameAsync(
            mapper.Map<Guid>(request.Id),
            request.Name,
            context.CancellationToken
        );

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
        await toDoService.UpdateToDoItemDescriptionAsync(
            mapper.Map<Guid>(request.Id),
            request.Description,
            context.CancellationToken
        );

        return new();
    }

    public override async Task<SkipToDoItemReply> SkipToDoItem(SkipToDoItemRequest request, ServerCallContext context)
    {
        await toDoService.SkipToDoItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new();
    }

    public override async Task<FailToDoItemReply> FailToDoItem(FailToDoItemRequest request, ServerCallContext context)
    {
        await toDoService.FailToDoItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new();
    }

    public override async Task<SearchToDoSubItemsReply> SearchToDoSubItems(
        SearchToDoSubItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.SearchToDoSubItemsAsync(request.SearchText, context.CancellationToken);
        var reply = new SearchToDoSubItemsReply();
        reply.Items.AddRange(items.Select(x => mapper.Map<ToDoSubItemGrpc>(x)));

        return reply;
    }

    public override async Task<UpdateToDoItemTypeReply> UpdateToDoItemType(
        UpdateToDoItemTypeRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemTypeAsync(
            mapper.Map<Guid>(request.Id),
            (ToDoItemType)request.Type,
            context.CancellationToken
        );

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

    public override async Task<GetFavoriteToDoItemsReply> GetFavoriteToDoItems(
        GetFavoriteToDoItemsRequest request,
        ServerCallContext context
    )
    {
        var reply = new GetFavoriteToDoItemsReply();
        var items = await toDoService.GetFavoriteToDoItemsAsync(context.CancellationToken);
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSubItemGrpc>>(items));

        return reply;
    }

    public override async Task<UpdateToDoItemAnnuallyPeriodicityReply> UpdateToDoItemAnnuallyPeriodicity(
        UpdateToDoItemAnnuallyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemAnnuallyPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<AnnuallyPeriodicity>(request.Periodicity),
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemMonthlyPeriodicityReply> UpdateToDoItemMonthlyPeriodicity(
        UpdateToDoItemMonthlyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemMonthlyPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<MonthlyPeriodicity>(request.Periodicity),
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemWeeklyPeriodicityReply> UpdateToDoItemWeeklyPeriodicity(
        UpdateToDoItemWeeklyPeriodicityRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemWeeklyPeriodicityAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<WeeklyPeriodicity>(request.Periodicity),
            context.CancellationToken
        );

        return new();
    }

    public override async Task<GetLeafToDoSubItemsReply> GetLeafToDoSubItems(
        GetLeafToDoSubItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.GetLeafToDoSubItemsAsync(
            mapper.Map<Guid>(request.Id),
            context.CancellationToken
        );
        var reply = new GetLeafToDoSubItemsReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSubItemGrpc>>(items));

        return reply;
    }

    public override async Task<GetToDoSelectorItemsReply> GetToDoSelectorItems(
        GetToDoSelectorItemsRequest request,
        ServerCallContext context
    )
    {
        var items = await toDoService.GetToDoSelectorItemsAsync(
            mapper.Map<Guid[]>(request.IgnoreIds),
            context.CancellationToken
        );
        var reply = new GetToDoSelectorItemsReply();
        reply.Items.AddRange(mapper.Map<IEnumerable<ToDoSelectorItemGrpc>>(items));

        return reply;
    }

    public override async Task<UpdateToDoItemParentReply> UpdateToDoItemParent(
        UpdateToDoItemParentRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemParentAsync(
            mapper.Map<Guid>(request.Id),
            mapper.Map<Guid>(request.ParentId),
            context.CancellationToken
        );

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

    public override async Task<ToDoItemToStringReply> ToDoItemToString(
        ToDoItemToStringRequest request,
        ServerCallContext context
    )
    {
        var value = await toDoService.ToDoItemToStringAsync(
            mapper.Map<ToDoItemToStringOptions>(request),
            context.CancellationToken
        );

        return new()
        {
            Value = value,
        };
    }

    public override async Task<UpdateToDoItemDaysOffsetReply> UpdateToDoItemDaysOffset(
        UpdateToDoItemDaysOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemDaysOffsetAsync(
            mapper.Map<Guid>(request.Id),
            (ushort)request.Days,
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemMonthsOffsetReply> UpdateToDoItemMonthsOffset(
        UpdateToDoItemMonthsOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemMonthsOffsetAsync(
            mapper.Map<Guid>(request.Id),
            (ushort)request.Months,
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemWeeksOffsetReply> UpdateToDoItemWeeksOffset(
        UpdateToDoItemWeeksOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemWeeksOffsetAsync(
            mapper.Map<Guid>(request.Id),
            (ushort)request.Weeks,
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemYearsOffsetReply> UpdateToDoItemYearsOffset(
        UpdateToDoItemYearsOffsetRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemYearsOffsetAsync(
            mapper.Map<Guid>(request.Id),
            (ushort)request.Years,
            context.CancellationToken
        );

        return new();
    }

    public override async Task<UpdateToDoItemChildrenTypeReply> UpdateToDoItemChildrenType(
        UpdateToDoItemChildrenTypeRequest request,
        ServerCallContext context
    )
    {
        await toDoService.UpdateToDoItemChildrenTypeAsync(
            mapper.Map<Guid>(request.Id),
            (ToDoItemChildrenType)request.Type,
            context.CancellationToken
        );

        return new();
    }
}