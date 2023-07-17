using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExtensionFramework.Core.Common.Extensions;
using Google.Protobuf;
using Spravy.Core.Enums;
using Spravy.Core.Interfaces;
using Spravy.Core.Models;
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

    public async Task<IEnumerable<ToDoSubItem>> GetRootToDoItemsAsync()
    {
        try
        {
            var items = await client.GetRootToDoItemsAsync(new GetRootToDoItemsRequest());

            return items.Items.Select(x => mapper.Map<ToDoSubItem>(x)).ToArray();
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task<ToDoItem> GetToDoItemAsync(Guid id)
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

            var result = mapper.Map<ToDoItem>(item.Item);

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

    public async Task UpdateTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        try
        {
            await client.UpdateTypeOfPeriodicityAsync(
                new UpdateTypeOfPeriodicityRequest
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

    public async Task UpdateDueDateAsync(Guid id, DateTimeOffset? dueDate)
    {
        try
        {
            await client.UpdateDueDateAsync(
                new UpdateDueDateRequest
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

    public async Task UpdateCompleteStatusAsync(Guid id, bool isComplete)
    {
        try
        {
            await client.UpdateCompleteStatusAsync(
                new UpdateCompleteStatusRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsComplete = isComplete
                }
            );
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateNameToDoItemAsync(Guid id, string name)
    {
        try
        {
            await client.UpdateNameToDoItemAsync(
                new UpdateNameToDoItemRequest
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

    public async Task UpdateOrderIndexToDoItemAsync(UpdateOrderIndexToDoItemOptions options)
    {
        try
        {
            await client.UpdateOrderIndexToDoItemAsync(mapper.Map<UpdateOrderIndexToDoItemRequest>(options));
        }
        catch (Exception e)
        {
            throw new GrpcException(grpcChannel.Target, e);
        }
    }

    public async Task UpdateDescriptionToDoItemAsync(Guid id, string description)
    {
        try
        {
            await client.UpdateDescriptionToDoItemAsync(
                new UpdateDescriptionToDoItemRequest
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
}