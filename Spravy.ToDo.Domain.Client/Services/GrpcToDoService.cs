using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Helpers;
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

    public Task ResetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new ResetToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.ResetToDoItemAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new RandomizeChildrenOrderIndexRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.RandomizeChildrenOrderIndexAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<ToDoShortItem>> GetParentsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetParentsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetParentsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<ToDoShortItem>>(reply.Parents);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<Guid>> SearchToDoItemIdsAsync(string searchText, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new SearchToDoItemIdsRequest
                {
                    SearchText = searchText,
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.SearchToDoItemIdsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<Guid>>(reply.Ids);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<Guid>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetLeafToDoItemIdsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetLeafToDoItemIdsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<Guid>>(reply.Ids);
            },
            cancellationToken
        );
    }

    public Task<ToDoItem> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();
                var reply = await client.GetToDoItemAsync(request, metadata, cancellationToken: cancellationToken);

                return mapper.Map<ToDoItem>(reply);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<Guid>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetChildrenToDoItemIdsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetChildrenToDoItemIdsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<Guid>>(reply.Ids);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<Guid>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetRootToDoItemIdsRequest();
                cancellationToken.ThrowIfCancellationRequested();
                var reply = await client.GetRootToDoItemIdsAsync(request, metadata);

                return mapper.Map<IEnumerable<Guid>>(reply.Ids);
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<Guid>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetFavoriteToDoItemIdsRequest();
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetFavoriteToDoItemIdsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<Guid>>(reply.Ids);
            },
            cancellationToken
        );
    }

    public Task<Guid> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = mapper.Map<AddRootToDoItemRequest>(options);
                cancellationToken.ThrowIfCancellationRequested();
                var id = await client.AddRootToDoItemAsync(request, metadata, cancellationToken: cancellationToken);

                return mapper.Map<Guid>(id.Id);
            },
            cancellationToken
        );
    }

    public Task<Guid> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = mapper.Map<AddToDoItemRequest>(options);
                cancellationToken.ThrowIfCancellationRequested();
                var id = await client.AddToDoItemAsync(request, metadata, cancellationToken: cancellationToken);

                return mapper.Map<Guid>(id.Id);
            },
            cancellationToken
        );
    }

    public Task DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new DeleteToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id)
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.DeleteToDoItemAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemTypeOfPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (TypeOfPeriodicityGrpc)type,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemTypeOfPeriodicityAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemDueDateRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    DueDate = mapper.Map<Timestamp>(dueDate),
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.UpdateToDoItemDueDateAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemCompleteStatusRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsCompleted = isComplete,
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.UpdateToDoItemCompleteStatusAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemNameRequest()
                {
                    Id = mapper.Map<ByteString>(id),
                    Name = name,
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.UpdateToDoItemNameAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = mapper.Map<UpdateToDoItemOrderIndexRequest>(options);
                cancellationToken.ThrowIfCancellationRequested();
                await client.UpdateToDoItemOrderIndexAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemDescriptionRequest()
                {
                    Description = description,
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.UpdateToDoItemDescriptionAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task SkipToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new SkipToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.SkipToDoItemAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task FailToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new FailToDoItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.FailToDoItemAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemTypeRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Type = (ToDoItemTypeGrpc)type,
                };

                cancellationToken.ThrowIfCancellationRequested();
                await client.UpdateToDoItemTypeAsync(request, metadata, cancellationToken: cancellationToken);
            },
            cancellationToken
        );
    }

    public Task AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.AddFavoriteToDoItemAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.RemoveFavoriteToDoItemAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemIsRequiredCompleteInDueDateAsync(Guid id, bool value, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdateToDoItemIsRequiredCompleteInDueDateRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsRequiredCompleteInDueDate = value,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<Guid>> GetTodayToDoItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetTodayToDoItemsRequest();

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetTodayToDoItemsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<Guid>>(reply.Ids);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemAnnuallyPeriodicityAsync(
                    new()
                    {
                        Periodicity = mapper.Map<AnnuallyPeriodicityGrpc>(periodicity),
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemMonthlyPeriodicityAsync(
                    new()
                    {
                        Periodicity = mapper.Map<MonthlyPeriodicityGrpc>(periodicity),
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemWeeklyPeriodicityAsync(
                    new()
                    {
                        Periodicity = mapper.Map<WeeklyPeriodicityGrpc>(periodicity),
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<ToDoSelectorItem>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetToDoSelectorItemsRequest();
                request.IgnoreIds.AddRange(mapper.Map<IEnumerable<ByteString>>(ignoreIds));
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetToDoSelectorItemsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<ToDoSelectorItem>>(reply.Items);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemParentAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        ParentId = mapper.Map<ByteString>(parentId),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.ToDoItemToRootAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<string> ToDoItemToStringAsync(ToDoItemToStringOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = mapper.Map<ToDoItemToStringRequest>(options);
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.ToDoItemToStringAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return reply.Value;
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemDaysOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Days = days
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemMonthsOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Months = months
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemWeeksOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Weeks = weeks
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemYearsOffsetAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Years = years
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemChildrenTypeAsync(Guid id, ToDoItemChildrenType type, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemChildrenTypeAsync(
                    new()
                    {
                        Id = mapper.Map<ByteString>(id),
                        Type = (ToDoItemChildrenTypeGrpc)type
                    },
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<ToDoShortItem>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                var request = new GetSiblingsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                var items = await client.GetSiblingsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<ToDoShortItem>>(items.Items);
            },
            cancellationToken
        );
    }

    public Task<ActiveToDoItem?> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = DefaultObject<GetCurrentActiveToDoItemRequest>.Default;
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetCurrentActiveToDoItemAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<ActiveToDoItem?>(reply.Item);
            },
            cancellationToken
        );
    }

    public Task UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var linkStr = mapper.Map<string>(link) ?? string.Empty;

                var request = new UpdateToDoItemLinkRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Link = linkStr,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdateToDoItemLinkAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<PlannedToDoItemSettings> GetPlannedToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetPlannedToDoItemSettingsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetPlannedToDoItemSettingsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<PlannedToDoItemSettings>(reply);
            },
            cancellationToken
        );
    }

    public Task<ValueToDoItemSettings> GetValueToDoItemSettingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetValueToDoItemSettingsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetValueToDoItemSettingsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<ValueToDoItemSettings>(reply);
            },
            cancellationToken
        );
    }

    public Task<PeriodicityToDoItemSettings> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetPeriodicityToDoItemSettingsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetPeriodicityToDoItemSettingsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<PeriodicityToDoItemSettings>(reply);
            },
            cancellationToken
        );
    }

    public Task<WeeklyPeriodicity> GetWeeklyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetWeeklyPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetWeeklyPeriodicityAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<WeeklyPeriodicity>(reply);
            },
            cancellationToken
        );
    }

    public Task<MonthlyPeriodicity> GetMonthlyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetMonthlyPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetMonthlyPeriodicityAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<MonthlyPeriodicity>(reply);
            },
            cancellationToken
        );
    }

    public Task<AnnuallyPeriodicity> GetAnnuallyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetAnnuallyPeriodicityRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetAnnuallyPeriodicityAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<AnnuallyPeriodicity>(reply);
            },
            cancellationToken
        );
    }

    public Task<PeriodicityOffsetToDoItemSettings> GetPeriodicityOffsetToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GetPeriodicityOffsetToDoItemSettingsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetPeriodicityOffsetToDoItemSettingsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<PeriodicityOffsetToDoItemSettings>(reply);
            },
            cancellationToken
        );
    }

    public IAsyncEnumerable<IEnumerable<ToDoItem>> GetToDoItemsAsync(
        Guid[] ids,
        uint chunkSize,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            (client, token) => GetToDoItemsAsyncCore(client, ids, chunkSize, token),
            cancellationToken
        );
    }

    private async IAsyncEnumerable<IEnumerable<ToDoItem>> GetToDoItemsAsyncCore(
        ToDoServiceClient client,
        Guid[] ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (!ids.Any())
        {
            yield return Enumerable.Empty<ToDoItem>();
            yield break;
        }

        var request = new GetToDoItemsRequest
        {
            ChunkSize = chunkSize,
        };

        request.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));
        cancellationToken.ThrowIfCancellationRequested();
        var metadata = await metadataFactory.CreateAsync(cancellationToken);
        using var response = client.GetToDoItems(request, metadata, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        while (await response.ResponseStream.MoveNext(cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var item = mapper.Map<IEnumerable<ToDoItem>>(reply.Items);

            yield return item;

            cancellationToken.ThrowIfCancellationRequested();
        }
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