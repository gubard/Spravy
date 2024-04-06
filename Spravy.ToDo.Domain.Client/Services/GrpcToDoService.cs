using System.Runtime.CompilerServices;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Spravy.Client.Extensions;
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

    public ConfiguredValueTaskAwaitable<Result> CloneToDoItemAsync(
        Guid cloneId,
        Guid? parentId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(cloneId),
                        converter.Convert<ByteString>(parentId),
                        (value, ci, pi) =>
                            client.CloneToDoItemAsync(
                                    new CloneToDoItemRequest
                                    {
                                        CloneId = ci,
                                        ParentId = pi,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionTypeAsync(
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
                        (value, i) => client.UpdateToDoItemDescriptionTypeAsync(
                                new UpdateToDoItemDescriptionTypeRequest
                                {
                                    Id = i,
                                    Type = (DescriptionTypeGrpc)type,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultOnly()
                            .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> ResetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) => client.ResetToDoItemAsync(
                            new ResetToDoItemRequest
                            {
                                Id = i,
                            },
                            value,
                            cancellationToken: cancellationToken
                        )
                        .ToValueTaskResultOnly()
                        .ConfigureAwait(false)
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> RandomizeChildrenOrderIndexAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) =>
                        client.RandomizeChildrenOrderIndexAsync(
                                new RandomizeChildrenOrderIndexRequest
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultOnly()
                            .ConfigureAwait(false)
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) => client.GetParentsAsync(
                            new GetParentsRequest
                            {
                                Id = i,
                            },
                            value,
                            cancellationToken: cancellationToken
                        )
                        .ToValueTaskResultValueOnly()
                        .ConfigureAwait(false)
                        .IfSuccessAsync(
                            reply => converter.Convert<ToDoShortItem[]>(reply.Parents)
                                .IfSuccess(p => p.ToReadOnlyMemory().ToResult())
                                .ToValueTaskResult()
                                .ConfigureAwait(false)
                        )
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        value => client.SearchToDoItemIdsAsync(
                                new SearchToDoItemIdsRequest
                                {
                                    SearchText = searchText,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<Guid[]>(reply.Ids)
                                    .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                    ),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) =>
                        client.GetLeafToDoItemIdsAsync(
                                new GetLeafToDoItemIdsRequest
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<Guid[]>(reply.Ids)
                                    .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ToDoItem>> GetToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) =>
                        client.GetToDoItemAsync(
                                new GetToDoItemRequest
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<ToDoItem>(reply).ToValueTaskResult().ConfigureAwait(false)
                            )
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) =>
                        client.GetChildrenToDoItemIdsAsync(
                                new GetChildrenToDoItemIdsRequest
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<Guid[]>(reply.Ids)
                                    .IfSuccess(
                                        ids => ids.ToReadOnlyMemory()
                                            .ToResult()
                                    )
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetChildrenToDoItemShortsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) =>
                        client.GetChildrenToDoItemShortsAsync(
                                new GetChildrenToDoItemShortsRequest
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<ToDoShortItem[]>(reply.Items)
                                    .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetRootToDoItemIdsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    value =>
                        client.GetRootToDoItemIdsAsync(new GetRootToDoItemIdsRequest(), value)
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<Guid[]>(reply.Ids)
                                    .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                ),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    value =>
                        client.GetFavoriteToDoItemIdsAsync(
                                new GetFavoriteToDoItemIdsRequest(),
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<Guid[]>(reply.Ids)
                                    .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                ),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddRootToDoItemAsync(
        AddRootToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .IfSuccessAsync(
                    converter.Convert<AddRootToDoItemRequest>(options),
                    (value, i) =>
                        client.AddRootToDoItemAsync(
                                i,
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                id => converter.Convert<Guid>(id.Id).ToValueTaskResult().ConfigureAwait(false)
                            )
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<Guid>> AddToDoItemAsync(
        AddToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<AddToDoItemRequest>(options),
                        (value, request) =>
                            client.AddToDoItemAsync(
                                    request,
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    id => converter.Convert<Guid>(id.Id).ToValueTaskResult().ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> DeleteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.DeleteToDoItemAsync(
                                    new DeleteToDoItemRequest
                                    {
                                        Id = i
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeOfPeriodicityAsync(
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
                        (value, i) =>
                            client.UpdateToDoItemTypeOfPeriodicityAsync(
                                    new UpdateToDoItemTypeOfPeriodicityRequest
                                    {
                                        Id = i,
                                        Type = (TypeOfPeriodicityGrpc)type,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDueDateAsync(
        Guid id,
        DateOnly dueDate,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<Timestamp>(dueDate),
                        (value, i, dd) =>
                            client.UpdateToDoItemDueDateAsync(
                                    new UpdateToDoItemDueDateRequest
                                    {
                                        Id = i,
                                        DueDate = dd,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemCompleteStatusAsync(
        Guid id,
        bool isComplete,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemCompleteStatusAsync(
                                    new UpdateToDoItemCompleteStatusRequest
                                    {
                                        Id = i,
                                        IsCompleted = isComplete,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemNameAsync(
                                    new UpdateToDoItemNameRequest()
                                    {
                                        Id = i,
                                        Name = name,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemOrderIndexAsync(
        UpdateOrderIndexToDoItemOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<UpdateToDoItemOrderIndexRequest>(options),
                        (value, request) =>
                            client.UpdateToDoItemOrderIndexAsync(
                                    request,
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDescriptionAsync(
        Guid id,
        string description,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemDescriptionAsync(
                                    new UpdateToDoItemDescriptionRequest
                                    {
                                        Description = description,
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemTypeAsync(
        Guid id,
        ToDoItemType type,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemTypeAsync(
                                    new UpdateToDoItemTypeRequest
                                    {
                                        Id = i,
                                        Type = (ToDoItemTypeGrpc)type,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> AddFavoriteToDoItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.AddFavoriteToDoItemAsync(
                                    new()
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> RemoveFavoriteToDoItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.RemoveFavoriteToDoItemAsync(
                                    new()
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemIsRequiredCompleteInDueDateAsync(
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
                        (metadata, i) =>
                            client.UpdateToDoItemIsRequiredCompleteInDueDateAsync(
                                    new UpdateToDoItemIsRequiredCompleteInDueDateRequest
                                    {
                                        Id = i,
                                        IsRequiredCompleteInDueDate = value,
                                    },
                                    metadata,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        value =>
                            client.GetTodayToDoItemsAsync(
                                    new GetTodayToDoItemsRequest(),
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<Guid[]>(reply.Ids)
                                        .IfSuccess(ids => ids.ToReadOnlyMemory().ToResult())
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    ),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemAnnuallyPeriodicityAsync(
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
                        (value, i, p) =>
                            client.UpdateToDoItemAnnuallyPeriodicityAsync(
                                    new()
                                    {
                                        Periodicity = p,
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthlyPeriodicityAsync(
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
                        (value, i, p) =>
                            client.UpdateToDoItemMonthlyPeriodicityAsync(
                                    new()
                                    {
                                        Periodicity = p,
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeklyPeriodicityAsync(
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
                        (value, i, p) =>
                            client.UpdateToDoItemWeeklyPeriodicityAsync(
                                    new()
                                    {
                                        Periodicity = p,
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoSelectorItem>>> GetToDoSelectorItemsAsync(
        Guid[] ignoreIds,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString[]>(ignoreIds),
                        (value, ii) =>
                        {
                            var request = new GetToDoSelectorItemsRequest();
                            request.IgnoreIds.AddRange(ii);

                            return client.GetToDoSelectorItemsAsync(
                                    request,
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<ToDoSelectorItem[]>(reply.Items)
                                        .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                );
                        }
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemParentAsync(
        Guid id,
        Guid parentId,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        converter.Convert<ByteString>(parentId),
                        (value, i, pi) =>
                            client.UpdateToDoItemParentAsync(
                                    new()
                                    {
                                        Id = i,
                                        ParentId = pi,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> ToDoItemToRootAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.ToDoItemToRootAsync(
                                    new()
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ToDoItemToStringOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ToDoItemToStringRequest>(options),
                        (value, request) =>
                            client.ToDoItemToStringAsync(
                                    request,
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Value.ToResult().ToValueTaskResult().ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemDaysOffsetAsync(
        Guid id,
        ushort days,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemDaysOffsetAsync(
                                    new()
                                    {
                                        Id = i,
                                        Days = days
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemMonthsOffsetAsync(
        Guid id,
        ushort months,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemMonthsOffsetAsync(
                                    new()
                                    {
                                        Id = i,
                                        Months = months
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemWeeksOffsetAsync(
        Guid id,
        ushort weeks,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemWeeksOffsetAsync(
                                    new()
                                    {
                                        Id = i,
                                        Weeks = weeks
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemYearsOffsetAsync(
        Guid id,
        ushort years,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdateToDoItemYearsOffsetAsync(
                                    new()
                                    {
                                        Id = i,
                                        Years = years
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemChildrenTypeAsync(
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
                        (value, i) =>
                            client.UpdateToDoItemChildrenTypeAsync(
                                    new()
                                    {
                                        Id = i,
                                        Type = (ToDoItemChildrenTypeGrpc)type
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetSiblingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetSiblingsAsync(
                                    new GetSiblingsRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    items => converter.Convert<ToDoShortItem[]>(items.Items)
                                        .IfSuccess(it => it.ToReadOnlyMemory().ToResult())
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ActiveToDoItem?>> GetCurrentActiveToDoItemAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        value =>
                            client.GetCurrentActiveToDoItemAsync(
                                    DefaultObject<GetCurrentActiveToDoItemRequest>.Default,
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<ActiveToDoItem?>(reply.Item)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    ),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdateToDoItemLinkAsync(
        Guid id,
        Uri? link,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<string?>(link),
                        converter.Convert<ByteString>(id),
                        (value, l, i) =>
                            client.UpdateToDoItemLinkAsync(
                                    new UpdateToDoItemLinkRequest
                                    {
                                        Id = i,
                                        Link = l ?? string.Empty,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<PlannedToDoItemSettings>> GetPlannedToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetPlannedToDoItemSettingsAsync(
                                    new GetPlannedToDoItemSettingsRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<PlannedToDoItemSettings>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ValueToDoItemSettings>> GetValueToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetValueToDoItemSettingsAsync(
                                    new GetValueToDoItemSettingsRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<ValueToDoItemSettings>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<PeriodicityToDoItemSettings>> GetPeriodicityToDoItemSettingsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetPeriodicityToDoItemSettingsAsync(
                                    new GetPeriodicityToDoItemSettingsRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<PeriodicityToDoItemSettings>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<WeeklyPeriodicity>> GetWeeklyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetWeeklyPeriodicityAsync(
                                    new GetWeeklyPeriodicityRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<WeeklyPeriodicity>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<MonthlyPeriodicity>> GetMonthlyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetMonthlyPeriodicityAsync(
                                    new GetMonthlyPeriodicityRequest
                                    {
                                        Id = i
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<MonthlyPeriodicity>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<AnnuallyPeriodicity>> GetAnnuallyPeriodicityAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetAnnuallyPeriodicityAsync(
                                    new GetAnnuallyPeriodicityRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<AnnuallyPeriodicity>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ConfiguredValueTaskAwaitable<Result<PeriodicityOffsetToDoItemSettings>>
        GetPeriodicityOffsetToDoItemSettingsAsync(
            Guid id,
            CancellationToken cancellationToken
        )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetPeriodicityOffsetToDoItemSettingsAsync(
                                    new GetPeriodicityOffsetToDoItemSettingsRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<PeriodicityOffsetToDoItemSettings>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                    )
                    .ConfigureAwait(false),
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

    private async ValueTask<bool> MoveNextAsync<T>(
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