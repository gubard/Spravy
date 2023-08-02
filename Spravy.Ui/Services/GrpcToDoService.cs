using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using Google.Protobuf;
using Spravy.Domain.Enums;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Protos;
using Spravy.Ui.Exceptions;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.Services;

public class GrpcToDoService : GrpcServiceBase, IToDoService
{
    private readonly ToDoService.ToDoServiceClient client;
    private readonly IMapper mapper;

    public GrpcToDoService(GrpcToDoServiceOptions options, IMapper mapper)
        : base(options.Host.ToUri(), options.ChannelType, options.ChannelCredentialType.GetChannelCredentials())
    {
        this.mapper = mapper;
        client = new ToDoService.ToDoServiceClient(grpcChannel);
    }

    public async Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync()
    {
        try
        {
            var items = await client.GetRootToDoSubItemsAsync(new GetRootToDoSubItemsRequest());

            return mapper.Map<IEnumerable<IToDoSubItem>>(items.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IToDoItem> GetToDoItemAsync(Guid id)
    {
        try
        {
            var byteStringId = mapper.Map<ByteString>(id);

            var item = await client.GetToDoItemAsync(
                new GetToDoItemRequest
                {
                    Id = byteStringId
                }
            );

            var result = mapper.Map<IToDoItem>(item.Item);

            return result;
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        try
        {
            var id = await client.AddRootToDoItemAsync(mapper.Map<AddRootToDoItemRequest>(options));

            return mapper.Map<Guid>(id.Id);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<Guid> AddToDoItemAsync(AddToDoItemOptions options)
    {
        try
        {
            var id = await client.AddToDoItemAsync(mapper.Map<AddToDoItemRequest>(options));

            return mapper.Map<Guid>(id.Id);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task DeleteToDoItemAsync(Guid id)
    {
        try
        {
            await client.DeleteToDoItemAsync(
                new DeleteToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id)
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        try
        {
            await client.UpdateToDoItemTypeOfPeriodicityAsync(
                new UpdateToDoItemTypeOfPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (TypeOfPeriodicityGrpc)type,
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemDueDateAsync(Guid id, DateTimeOffset dueDate)
    {
        try
        {
            await client.UpdateToDoItemDueDateAsync(
                new UpdateToDoItemDueDateRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    DueDate = mapper.Map<DateTimeOffsetGrpc>(dueDate),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted)
    {
        try
        {
            await client.UpdateToDoItemCompleteStatusAsync(
                new UpdateToDoItemCompleteStatusRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsCompleted = isCompleted
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemNameAsync(Guid id, string name)
    {
        try
        {
            await client.UpdateToDoItemNameAsync(
                new UpdateToDoItemNameRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Name = name,
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        try
        {
            await client.UpdateToDoItemOrderIndexAsync(mapper.Map<UpdateToDoItemOrderIndexRequest>(options));
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemDescriptionAsync(Guid id, string description)
    {
        try
        {
            await client.UpdateToDoItemDescriptionAsync(
                new UpdateToDoItemDescriptionRequest
                {
                    Description = description,
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task SkipToDoItemAsync(Guid id)
    {
        try
        {
            await client.SkipToDoItemAsync(
                new SkipToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task FailToDoItemAsync(Guid id)
    {
        try
        {
            await client.FailToDoItemAsync(
                new FailToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText)
    {
        try
        {
            var reply = await client.SearchToDoSubItemsAsync(
                new SearchToDoSubItemsRequest
                {
                    SearchText = searchText
                }
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type)
    {
        try
        {
            await client.UpdateToDoItemTypeAsync(
                new UpdateToDoItemTypeRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (ToDoItemTypeGrpc)type,
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task AddCurrentToDoItemAsync(Guid id)
    {
        try
        {
            await client.AddCurrentToDoItemAsync(
                new AddCurrentToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task RemoveCurrentToDoItemAsync(Guid id)
    {
        try
        {
            await client.RemoveCurrentToDoItemAsync(
                new RemoveCurrentToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> GetCurrentToDoItemsAsync()
    {
        try
        {
            var reply = await client.GetCurrentToDoItemsAsync(new GetCurrentToDoItemsRequest());

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity)
    {
        try
        {
            await client.UpdateToDoItemAnnuallyPeriodicityAsync(
                new UpdateToDoItemAnnuallyPeriodicityRequest
                {
                    Periodicity = mapper.Map<AnnuallyPeriodicityGrpc>(periodicity),
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity)
    {
        try
        {
            await client.UpdateToDoItemMonthlyPeriodicityAsync(
                new UpdateToDoItemMonthlyPeriodicityRequest
                {
                    Periodicity = mapper.Map<MonthlyPeriodicityGrpc>(periodicity),
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity)
    {
        try
        {
            await client.UpdateToDoItemWeeklyPeriodicityAsync(
                new UpdateToDoItemWeeklyPeriodicityRequest
                {
                    Periodicity = mapper.Map<WeeklyPeriodicityGrpc>(periodicity),
                    Id = mapper.Map<ByteString>(id),
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id)
    {
        try
        {
            var reply = await client.GetLeafToDoSubItemsAsync(
                new GetLeafToDoSubItemsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                }
            );

            return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }
}