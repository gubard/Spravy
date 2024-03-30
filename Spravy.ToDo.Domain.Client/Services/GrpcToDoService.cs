using System.Runtime.CompilerServices;
using AutoMapper;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Enums;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.ToDo.Domain.Models;
using Spravy.ToDo.Protos;
using static Spravy.ToDo.Protos.ToDoService;

namespace Spravy.ToDo.Domain.Client.Services;

public class GrpcToDoService : GrpcServiceBase<ToDoServiceClient>,
    IToDoService,
    IGrpcServiceCreatorAuth<GrpcToDoService, ToDoServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public Task<Result> CloneToDoItemAsync(Guid cloneId, Guid? parentId, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new CloneToDoItemRequest
                            {
                                CloneId = mapper.Map<ByteString>(cloneId),
                                ParentId = mapper.Map<ByteString>(parentId),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.CloneToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDescriptionTypeAsync(
        Guid id,
        DescriptionType type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemDescriptionTypeRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Type = (DescriptionTypeGrpc)type,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemDescriptionTypeAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> ResetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new ResetToDoItemRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.ResetToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new RandomizeChildrenOrderIndexRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.RandomizeChildrenOrderIndexAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetParentsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetParentsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<ToDoShortItem>>(reply.Parents).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new SearchToDoItemIdsRequest
                            {
                                SearchText = searchText,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.SearchToDoItemIdsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<Guid>>(reply.Ids).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetLeafToDoItemIdsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetLeafToDoItemIdsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<Guid>>(reply.Ids).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetToDoItemRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            var reply = await client.GetToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ToDoItem>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetChildrenToDoItemIdsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetChildrenToDoItemIdsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<Guid>>(reply.Ids).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetChildrenToDoItemShortsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetChildrenToDoItemShortsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<ToDoShortItem>>(reply.Items).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetRootToDoItemIdsRequest();
                            cancellationToken.ThrowIfCancellationRequested();
                            var reply = await client.GetRootToDoItemIdsAsync(request, value);

                            return mapper.Map<ReadOnlyMemory<Guid>>(reply.Ids).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetFavoriteToDoItemIdsRequest();
                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetFavoriteToDoItemIdsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<Guid>>(reply.Ids).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<Guid>> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = mapper.Map<AddRootToDoItemRequest>(options);
                            cancellationToken.ThrowIfCancellationRequested();
                            var id = await client.AddRootToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<Guid>(id.Id).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<Guid>> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = mapper.Map<AddToDoItemRequest>(options);
                            cancellationToken.ThrowIfCancellationRequested();

                            var id = await client.AddToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<Guid>(id.Id).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new DeleteToDoItemRequest
                            {
                                Id = mapper.Map<ByteString>(id)
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.DeleteToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemTypeOfPeriodicityAsync(
        Guid id,
        TypeOfPeriodicity type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemTypeOfPeriodicityRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Type = (TypeOfPeriodicityGrpc)type,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemTypeOfPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemDueDateRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                DueDate = mapper.Map<Timestamp>(dueDate),
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            await client.UpdateToDoItemDueDateAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemCompleteStatusRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                IsCompleted = isComplete,
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            await client.UpdateToDoItemCompleteStatusAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemNameRequest()
                            {
                                Id = mapper.Map<ByteString>(id),
                                Name = name,
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            await client.UpdateToDoItemNameAsync(request, value, cancellationToken: cancellationToken);

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = mapper.Map<UpdateToDoItemOrderIndexRequest>(options);
                            cancellationToken.ThrowIfCancellationRequested();
                            await client.UpdateToDoItemOrderIndexAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemDescriptionRequest()
                            {
                                Description = description,
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            await client.UpdateToDoItemDescriptionAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdateToDoItemTypeRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Type = (ToDoItemTypeGrpc)type,
                            };

                            cancellationToken.ThrowIfCancellationRequested();
                            await client.UpdateToDoItemTypeAsync(request, value, cancellationToken: cancellationToken);

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.AddFavoriteToDoItemAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.RemoveFavoriteToDoItemAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
        Guid id,
        bool value,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async metadata =>
                        {
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

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetTodayToDoItemsRequest();

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetTodayToDoItemsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<Guid>>(reply.Ids).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
        Guid id,
        AnnuallyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemAnnuallyPeriodicityAsync(
                                new()
                                {
                                    Periodicity = mapper.Map<AnnuallyPeriodicityGrpc>(periodicity),
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemMonthlyPeriodicityAsync(
        Guid id,
        MonthlyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemMonthlyPeriodicityAsync(
                                new()
                                {
                                    Periodicity = mapper.Map<MonthlyPeriodicityGrpc>(periodicity),
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemWeeklyPeriodicityAsync(
        Guid id,
        WeeklyPeriodicity periodicity,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemWeeklyPeriodicityAsync(
                                new()
                                {
                                    Periodicity = mapper.Map<WeeklyPeriodicityGrpc>(periodicity),
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetToDoSelectorItemsRequest();
                            request.IgnoreIds.AddRange(mapper.Map<IEnumerable<ByteString>>(ignoreIds));
                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetToDoSelectorItemsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<ToDoSelectorItem>>(reply.Items).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemParentAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                    ParentId = mapper.Map<ByteString>(parentId),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );
                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.ToDoItemToRootAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = mapper.Map<ToDoItemToStringRequest>(options);
                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.ToDoItemToStringAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return reply.Value.ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemDaysOffsetAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                    Days = days
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemMonthsOffsetAsync(Guid id, ushort months, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemMonthsOffsetAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                    Months = months
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemWeeksOffsetAsync(Guid id, ushort weeks, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemWeeksOffsetAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                    Weeks = weeks
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemYearsOffsetAsync(Guid id, ushort years, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemYearsOffsetAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                    Years = years
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemChildrenTypeAsync(
        Guid id,
        ToDoItemChildrenType type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemChildrenTypeAsync(
                                new()
                                {
                                    Id = mapper.Map<ByteString>(id),
                                    Type = (ToDoItemChildrenTypeGrpc)type
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var request = new GetSiblingsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            var items = await client.GetSiblingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<ToDoShortItem>>(items.Items).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = DefaultObject<GetCurrentActiveToDoItemRequest>.Default;
                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetCurrentActiveToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ActiveToDoItem?>(reply.Item).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var linkStr = mapper.Map<string>(link) ?? string.Empty;

                            var request = new UpdateToDoItemLinkRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Link = linkStr,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemLinkAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetPlannedToDoItemSettingsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetPlannedToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<PlannedToDoItemSettings>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetValueToDoItemSettingsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetValueToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ValueToDoItemSettings>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetPeriodicityToDoItemSettingsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetPeriodicityToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<PeriodicityToDoItemSettings>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetWeeklyPeriodicityRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetWeeklyPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<WeeklyPeriodicity>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetMonthlyPeriodicityRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetMonthlyPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<MonthlyPeriodicity>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetAnnuallyPeriodicityRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetAnnuallyPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<AnnuallyPeriodicity>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<PeriodicityOffsetToDoItemSettings>> GetPeriodicityOffsetToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetPeriodicityOffsetToDoItemSettingsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetPeriodicityOffsetToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<PeriodicityOffsetToDoItemSettings>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public IAsyncEnumerable<ReadOnlyMemory<ToDoItem>> GetToDoItemsAsync(
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

    private async IAsyncEnumerable<ReadOnlyMemory<ToDoItem>> GetToDoItemsAsyncCore(
        ToDoServiceClient client,
        Guid[] ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (!ids.Any())
        {
            yield return ReadOnlyMemory<ToDoItem>.Empty;
            yield break;
        }

        var request = new GetToDoItemsRequest
        {
            ChunkSize = chunkSize,
        };

        request.Ids.AddRange(mapper.Map<IEnumerable<ByteString>>(ids));
        cancellationToken.ThrowIfCancellationRequested();
        var metadata = await metadataFactory.CreateAsync(cancellationToken);
        using var response = client.GetToDoItems(request, metadata.Value, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        while (await MoveNextAsync(response, cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var item = mapper.Map<ReadOnlyMemory<ToDoItem>>(reply.Items);

            yield return item;

            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private async Task<bool> MoveNextAsync<T>(
        AsyncServerStreamingCall<T> streamingCall,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await streamingCall.ResponseStream.MoveNext(cancellationToken);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
            return false;
        }
    }

    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory, serializer);
    }
}