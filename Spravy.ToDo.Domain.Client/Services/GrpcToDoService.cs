using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;
using static Spravy.ToDo.Protos.ToDoService;

namespace Spravy.ToDo.Domain.Client.Services;

public class GrpcToDoService : GrpcServiceBase<ToDoServiceClient>,
    IToDoService,
    IGrpcServiceCreator<GrpcToDoService, ToDoServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    ) : base(grpcClientFactory, host)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public Task<IEnumerable<IToDoSubItem>> GetRootToDoSubItemsAsync(TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var metadata = await metadataFactory.CreateAsync();

                var request = new GetRootToDoSubItemsRequest
                {
                    Offset = mapper.Map<Duration>(offset),
                };

                var items = await client.GetRootToDoSubItemsAsync(request, metadata);

                return mapper.Map<IEnumerable<IToDoSubItem>>(items.Items);
            }
        );
    }

    public Task<IToDoItem> GetToDoItemAsync(Guid id, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var byteStringId = mapper.Map<ByteString>(id);

                var request = new GetToDoItemRequest
                {
                    Id = byteStringId,
                    Offset = mapper.Map<Duration>(offset),
                };

                var item = await client.GetToDoItemAsync(request, await metadataFactory.CreateAsync());
                var result = mapper.Map<IToDoItem>(item.Item);

                return result;
            }
        );
    }


    public Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options)
    {
        return CallClientAsync(
            async client =>
            {
                var request = mapper.Map<AddRootToDoItemRequest>(options);
                var id = await client.AddRootToDoItemAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<Guid>(id.Id);
            }
        );
    }

    public Task<Guid> AddToDoItemAsync(AddToDoItemOptions options, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = mapper.Map<AddToDoItemRequest>(options);
                request.Offset = mapper.Map<Duration>(offset);
                var id = await client.AddToDoItemAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<Guid>(id.Id);
            }
        );
    }

    public Task DeleteToDoItemAsync(Guid id)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new DeleteToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id)
                };

                await client.DeleteToDoItemAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task UpdateToDoItemTypeOfPeriodicityAsync(Guid id, TypeOfPeriodicity type)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new UpdateToDoItemTypeOfPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (TypeOfPeriodicityGrpc)type,
                };

                await client.UpdateToDoItemTypeOfPeriodicityAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new UpdateToDoItemDueDateRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    DueDate = mapper.Map<Timestamp>(dueDate),
                };

                await client.UpdateToDoItemDueDateAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isCompleted, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new UpdateToDoItemCompleteStatusRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsCompleted = isCompleted,
                    Offset = mapper.Map<Duration>(offset),
                };

                await client.UpdateToDoItemCompleteStatusAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task UpdateToDoItemNameAsync(Guid id, string name)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemNameAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Name = name,
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemOrderIndexAsync(UpdateOrderIndexToDoItemOptions options)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemOrderIndexAsync(
                    mapper.Map<UpdateToDoItemOrderIndexRequest>(options),
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemDescriptionAsync(Guid id, string description)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemDescriptionAsync(
                    new()
                    {
                        Description = description,
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task SkipToDoItemAsync(Guid id, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new SkipToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Offset = mapper.Map<Duration>(offset),
                };

                await client.SkipToDoItemAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task FailToDoItemAsync(Guid id, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new FailToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Offset = mapper.Map<Duration>(offset),
                };

                await client.FailToDoItemAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public Task<IEnumerable<IToDoSubItem>> SearchToDoSubItemsAsync(string searchText, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new SearchToDoSubItemsRequest
                {
                    SearchText = searchText,
                    Offset = mapper.Map<Duration>(offset),
                };

                var reply = await client.SearchToDoSubItemsAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
            }
        );
    }

    public Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type)
    {
        return CallClientAsync(
            async client =>
            {
                var metadata = await metadataFactory.CreateAsync();

                var request = new UpdateToDoItemTypeRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (ToDoItemTypeGrpc)type,
                };

                await client.UpdateToDoItemTypeAsync(request, metadata);
            }
        );
    }

    public Task AddFavoriteToDoItemAsync(Guid id)
    {
        return CallClientAsync(
            async client =>
            {
                await client.AddFavoriteToDoItemAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task RemoveFavoriteToDoItemAsync(Guid id)
    {
        return CallClientAsync(
            async client =>
            {
                await client.RemoveFavoriteToDoItemAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task<IEnumerable<IToDoSubItem>> GetFavoriteToDoItemsAsync(TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetFavoriteToDoItemsRequest
                {
                    Offset = mapper.Map<Duration>(offset),
                };

                var reply = await client.GetFavoriteToDoItemsAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
            }
        );
    }

    public Task UpdateToDoItemAnnuallyPeriodicityAsync(Guid id, AnnuallyPeriodicity periodicity)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemAnnuallyPeriodicityAsync(
                    new()
                    {
                        Periodicity = mapper.Map<AnnuallyPeriodicityGrpc>(periodicity),
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemMonthlyPeriodicityAsync(Guid id, MonthlyPeriodicity periodicity)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemMonthlyPeriodicityAsync(
                    new()
                    {
                        Periodicity = mapper.Map<MonthlyPeriodicityGrpc>(periodicity),
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemWeeklyPeriodicityAsync(Guid id, WeeklyPeriodicity periodicity)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemWeeklyPeriodicityAsync(
                    new()
                    {
                        Periodicity = mapper.Map<WeeklyPeriodicityGrpc>(periodicity),
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task<IEnumerable<IToDoSubItem>> GetLeafToDoSubItemsAsync(Guid id, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetLeafToDoSubItemsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Offset = mapper.Map<Duration>(offset),
                };

                var reply = await client.GetLeafToDoSubItemsAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<IToDoSubItem>>(reply.Items);
            }
        );
    }

    public Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(Guid[] ignoreIds)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetToDoSelectorItemsRequest();
                request.IgnoreIds.AddRange(mapper.Map<IEnumerable<ByteString>>(ignoreIds));
                var reply = await client.GetToDoSelectorItemsAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<ToDoSelectorItem>>(reply.Items);
            }
        );
    }

    public Task UpdateToDoItemParentAsync(Guid id, Guid parentId)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemParentAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        ParentId = mapper.Map<ByteString>(parentId),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task ToDoItemToRootAsync(Guid id)
    {
        return CallClientAsync(
            async client =>
            {
                await client.ToDoItemToRootAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options, TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = mapper.Map<ToDoItemToStringRequest>(options);
                request.Offset = mapper.Map<Duration>(offset);
                var reply = await client.ToDoItemToStringAsync(request, await metadataFactory.CreateAsync());

                return reply.Value;
            }
        );
    }

    public Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemDaysOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Days = days
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemMonthsOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Months = months
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemWeeksOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Weeks = weeks
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemYearsOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Years = years
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type)
    {
        return CallClientAsync(
            async client =>
            {
                await client.UpdateToDoItemChildrenTypeAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Type = (ToDoItemChildrenTypeGrpc)type
                    },
                    await metadataFactory.CreateAsync()
                );
            }
        );
    }

    public Task<IEnumerable<ToDoShortItem>> GetSiblingsAsync(Guid id)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetSiblingsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                var items = await client.GetSiblingsAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<IEnumerable<ToDoShortItem>>(items.Items);
            }
        );
    }

    public Task<ActiveToDoItem?> GetActiveToDoItemAsync(TimeSpan offset)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new GetActiveItemRequest
                {
                    Offset = mapper.Map<Duration>(offset),
                };

                var reply = await client.GetActiveItemAsync(request, await metadataFactory.CreateAsync());

                return mapper.Map<ActiveToDoItem?>(reply.Item);
            }
        );
    }

    public Task UpdateToDoItemLinkAsync(Guid id, Uri? link)
    {
        return CallClientAsync(
            async client =>
            {
                var request = new UpdateToDoItemLinkRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Link = mapper.Map<string>(link),
                };

                await client.UpdateToDoItemLinkAsync(request, await metadataFactory.CreateAsync());
            }
        );
    }

    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory);
    }
}