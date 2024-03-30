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
    private readonly IConverter converter;
    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        IFactory<Uri, ToDoServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.converter = converter;
        this.metadataFactory = metadataFactory;
    }

    public Task<Result> CloneToDoItemAsync(Guid cloneId, Guid? parentId, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(cloneId),
                        converter.Convert<ByteString>(parentId),
                        async (value, ci, pi) =>
                        {
                            var request = new CloneToDoItemRequest
                            {
                                CloneId = ci,
                                ParentId = pi,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.CloneToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdateToDoItemDescriptionTypeRequest
                            {
                                Id = i,
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
                    ),
            cancellationToken
        );
    }

    public Task<Result> ResetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new ResetToDoItemRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();

                        await client.ResetToDoItemAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return Result.Success;
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result> RandomizeChildrenOrderIndexAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new RandomizeChildrenOrderIndexRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();

                        await client.RandomizeChildrenOrderIndexAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return Result.Success;
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new GetParentsRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();

                        var reply = await client.GetParentsAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<ToDoShortItem[]>(reply.Parents)
                            .IfSuccess(p => p.ToReadOnlyMemory().ToResult());
                    }
                ),
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
                metadataFactory.CreateAsync(cancellationToken)
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

                            return converter.Convert<ReadOnlyMemory<Guid>>(reply.Ids);
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new GetLeafToDoItemIdsRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();

                        var reply = await client.GetLeafToDoItemIdsAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<ReadOnlyMemory<Guid>>(reply.Ids);
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new GetToDoItemRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();
                        var reply = await client.GetToDoItemAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<ToDoItem>(reply);
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new GetChildrenToDoItemIdsRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();

                        var reply = await client.GetChildrenToDoItemIdsAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<Guid[]>(reply.Ids)
                            .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult());
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    async (value, i) =>
                    {
                        var request = new GetChildrenToDoItemShortsRequest
                        {
                            Id = i,
                        };

                        cancellationToken.ThrowIfCancellationRequested();

                        var reply = await client.GetChildrenToDoItemShortsAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<ToDoShortItem[]>(reply.Items)
                            .IfSuccess(items => items.ToReadOnlyMemory().ToResult());
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    async value =>
                    {
                        var request = new GetRootToDoItemIdsRequest();
                        cancellationToken.ThrowIfCancellationRequested();
                        var reply = await client.GetRootToDoItemIdsAsync(request, value);

                        return converter.Convert<Guid[]>(reply.Ids)
                            .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult());
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    async value =>
                    {
                        var request = new GetFavoriteToDoItemIdsRequest();

                        var reply = await client.GetFavoriteToDoItemIdsAsync(
                            request,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<Guid[]>(reply.Ids)
                            .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult());
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<Guid>> AddRootToDoItemAsync(AddRootToDoItemOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<AddRootToDoItemRequest>(options),
                    async (value, i) =>
                    {
                        var id = await client.AddRootToDoItemAsync(
                            i,
                            value,
                            cancellationToken: cancellationToken
                        );

                        return converter.Convert<Guid>(id.Id);
                    }
                ),
            cancellationToken
        );
    }

    public Task<Result<Guid>> AddToDoItemAsync(AddToDoItemOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<AddToDoItemRequest>(options),
                        async (value, request) =>
                        {
                            var id = await client.AddToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<Guid>(id.Id);
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new DeleteToDoItemRequest
                            {
                                Id = i
                            };

                            await client.DeleteToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdateToDoItemTypeOfPeriodicityRequest
                            {
                                Id = i,
                                Type = (TypeOfPeriodicityGrpc)type,
                            };

                            await client.UpdateToDoItemTypeOfPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDueDateAsync(Guid id, DateOnly dueDate, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<Timestamp>(dueDate),
                        async (value, i, dd) =>
                        {
                            var request = new UpdateToDoItemDueDateRequest
                            {
                                Id = i,
                                DueDate = dd,
                            };

                            await client.UpdateToDoItemDueDateAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemCompleteStatusAsync(Guid id, bool isComplete, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdateToDoItemCompleteStatusRequest
                            {
                                Id = i,
                                IsCompleted = isComplete,
                            };

                            await client.UpdateToDoItemCompleteStatusAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdateToDoItemNameRequest()
                            {
                                Id = i,
                                Name = name,
                            };

                            await client.UpdateToDoItemNameAsync(request, value, cancellationToken: cancellationToken);

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<UpdateToDoItemOrderIndexRequest>(options),
                        async (value, request) =>
                        {
                            await client.UpdateToDoItemOrderIndexAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDescriptionAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdateToDoItemDescriptionRequest
                            {
                                Description = description,
                                Id = i,
                            };

                            await client.UpdateToDoItemDescriptionAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemTypeAsync(Guid id, ToDoItemType type, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdateToDoItemTypeRequest
                            {
                                Id = i,
                                Type = (ToDoItemTypeGrpc)type,
                            };

                            await client.UpdateToDoItemTypeAsync(request, value, cancellationToken: cancellationToken);

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.AddFavoriteToDoItemAsync(
                                new()
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> RemoveFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.RemoveFavoriteToDoItemAsync(
                                new()
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (metadata, i) =>
                        {
                            var request = new UpdateToDoItemIsRequiredCompleteInDueDateRequest
                            {
                                Id = i,
                                IsRequiredCompleteInDueDate = value,
                            };

                            await client.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                                request,
                                metadata,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetTodayToDoItemsRequest();

                            var reply = await client.GetTodayToDoItemsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<Guid[]>(reply.Ids)
                                .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult());
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<AnnuallyPeriodicityGrpc>(periodicity),
                        async (value, i, p) =>
                        {
                            await client.UpdateToDoItemAnnuallyPeriodicityAsync(
                                new()
                                {
                                    Periodicity = p,
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<MonthlyPeriodicityGrpc>(periodicity),
                        async (value, i, p) =>
                        {
                            await client.UpdateToDoItemMonthlyPeriodicityAsync(
                                new()
                                {
                                    Periodicity = p,
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<WeeklyPeriodicityGrpc>(periodicity),
                        async (value, i, p) =>
                        {
                            await client.UpdateToDoItemWeeklyPeriodicityAsync(
                                new()
                                {
                                    Periodicity = p,
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString[]>(ignoreIds),
                        async (value, ii) =>
                        {
                            var request = new GetToDoSelectorItemsRequest();
                            request.IgnoreIds.AddRange(ii);

                            var reply = await client.GetToDoSelectorItemsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<ToDoSelectorItem[]>(reply.Items)
                                .IfSuccess(items => items.ToReadOnlyMemory().ToResult());
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemParentAsync(Guid id, Guid parentId, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<ByteString>(parentId),
                        async (value, i, pi) =>
                        {
                            await client.UpdateToDoItemParentAsync(
                                new()
                                {
                                    Id = i,
                                    ParentId = pi,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.ToDoItemToRootAsync(
                                new()
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ToDoItemToStringRequest>(options),
                        async (value, request) =>
                        {
                            var reply = await client.ToDoItemToStringAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return reply.Value.ToResult();
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemDaysOffsetAsync(Guid id, ushort days, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.UpdateToDoItemDaysOffsetAsync(
                                new()
                                {
                                    Id = i,
                                    Days = days
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.UpdateToDoItemMonthsOffsetAsync(
                                new()
                                {
                                    Id = i,
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.UpdateToDoItemWeeksOffsetAsync(
                                new()
                                {
                                    Id = i,
                                    Weeks = weeks
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            await client.UpdateToDoItemYearsOffsetAsync(
                                new()
                                {
                                    Id = i,
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdateToDoItemChildrenTypeAsync(
                                new()
                                {
                                    Id = i,
                                    Type = (ToDoItemChildrenTypeGrpc)type
                                },
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var request = new GetSiblingsRequest
                            {
                                Id = i,
                            };

                            var items = await client.GetSiblingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<ToDoShortItem[]>(items.Items)
                                .IfSuccess(it => it.ToReadOnlyMemory().ToResult());
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = DefaultObject<GetCurrentActiveToDoItemRequest>.Default;

                            var reply = await client.GetCurrentActiveToDoItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<ActiveToDoItem?>(reply.Item);
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> UpdateToDoItemLinkAsync(Guid id, Uri? link, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<string?>(link),
                        converter.Convert<ByteString>(id),
                        async (value, l, i) =>
                        {
                            var linkStr = l ?? string.Empty;

                            var request = new UpdateToDoItemLinkRequest
                            {
                                Id = i,
                                Link = linkStr,
                            };

                            await client.UpdateToDoItemLinkAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return Result.Success;
                        }
                    ),
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetPlannedToDoItemSettingsRequest
                            {
                                Id = i,
                            };

                            var reply = await client.GetPlannedToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<PlannedToDoItemSettings>(reply);
                        }
                    ),
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetValueToDoItemSettingsRequest
                            {
                                Id = i,
                            };

                            var reply = await client.GetValueToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<ValueToDoItemSettings>(reply);
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
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetPeriodicityToDoItemSettingsRequest
                            {
                                Id = i,
                            };

                            var reply = await client.GetPeriodicityToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<PeriodicityToDoItemSettings>(reply);
                        }
                    ),
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetWeeklyPeriodicityRequest
                            {
                                Id = i,
                            };

                            var reply = await client.GetWeeklyPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<WeeklyPeriodicity>(reply);
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetMonthlyPeriodicityRequest
                            {
                                Id = i
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetMonthlyPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<MonthlyPeriodicity>(reply);
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetAnnuallyPeriodicityRequest
                            {
                                Id = i,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetAnnuallyPeriodicityAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<AnnuallyPeriodicity>(reply);
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
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetPeriodicityOffsetToDoItemSettingsRequest
                            {
                                Id = i,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetPeriodicityOffsetToDoItemSettingsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<PeriodicityOffsetToDoItemSettings>(reply);
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

        request.Ids.AddRange(converter.Convert<ByteString[]>(ids).Value);
        cancellationToken.ThrowIfCancellationRequested();
        var metadata = await metadataFactory.CreateAsync(cancellationToken);
        using var response = client.GetToDoItems(request, metadata.Value, cancellationToken: cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        while (await MoveNextAsync(response, cancellationToken))
        {
            var reply = response.ResponseStream.Current;
            var item = converter.Convert<ToDoItem[]>(reply.Items);

            yield return item.Value;

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
        IConverter mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory, serializer);
    }
}