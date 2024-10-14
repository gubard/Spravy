using Spravy.Domain.Helpers;
using Spravy.ToDo.Domain.Mapper.Mappers;
using Spravy.ToDo.Domain.Models;

namespace Spravy.ToDo.Domain.Client.Services;

public class GrpcToDoService
    : GrpcServiceBase<ToDoService.ToDoServiceClient>,
        IToDoService,
        IGrpcServiceCreatorAuth<GrpcToDoService, ToDoService.ToDoServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    private readonly IMetadataFactory metadataFactory;

    public GrpcToDoService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
        : base(grpcClientFactory, host, handler, retryService)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcToDoService CreateGrpcService(
        IFactory<Uri, ToDoService.ToDoServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        return new(grpcClientFactory, host, metadataFactory, handler, retryService);
    }

    public ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetActiveToDoItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetActiveToDoItemAsync(
                                    new() { Id = id.ToByteString() },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Item.ToOptionActiveToDoItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> CloneToDoItemAsync(
        ReadOnlyMemory<Guid> cloneIds,
        OptionStruct<Guid> parentId,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new CloneToDoItemRequest
                            {
                                ParentId = parentId.ToByteString()
                            };

                            request.CloneIds.AddRange(
                                cloneIds.Select(x => x.ToByteString()).ToArray()
                            );

                            return client
                                .CloneToDoItemAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply =>
                                        reply
                                            .NewItemIds.Select(x => x.ToGuid())
                                            .ToArray()
                                            .ToReadOnlyMemory()
                                            .ToResult(),
                                    ct
                                );
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar ResetToDoItemAsync(
        ReadOnlyMemory<ResetToDoItemOptions> options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new ResetToDoItemRequest();

                            request.Items.AddRange(
                                options.Select(x => x.ToResetToDoItemOptionsGrpc()).ToArray()
                            );

                            return client
                                .ResetToDoItemAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false);
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar EditToDoItemsAsync(EditToDoItems options, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .EditToDoItemsAsync(
                                    new() { Value = options.ToEditToDoItemsGrpc() },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<bool>> UpdateEventsAsync(CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .UpdateEventsAsync(new(), metadata, cancellationToken: ct)
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.IsUpdated.ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public Cvtar SwitchCompleteAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new SwitchCompleteRequest();
                            request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                            return client
                                .SwitchCompleteAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false);
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar UpdateToDoItemOrderIndexAsync(
        ReadOnlyMemory<UpdateOrderIndexToDoItemOptions> options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new UpdateToDoItemOrderIndexRequest();

                            request.Items.AddRange(
                                options
                                    .Select(x => x.ToUpdateOrderIndexToDoItemOptionsGrpc())
                                    .ToArray()
                            );

                            return client
                                .UpdateToDoItemOrderIndexAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false);
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetBookmarkToDoItemIdsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetBookmarkToDoItemIdsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public Cvtar RandomizeChildrenOrderIndexAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new RandomizeChildrenOrderIndexRequest();
                            request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                            return client
                                .RandomizeChildrenOrderIndexAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false);
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<ToDoShortItem>>> GetParentsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetParentsAsync(
                                    new() { Id = id.ToByteString() },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Parents.ToToDoShortItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> SearchToDoItemIdsAsync(
        string searchText,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .SearchToDoItemIdsAsync(
                                    new() { SearchText = searchText },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetLeafToDoItemIdsAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetLeafToDoItemIdsAsync(
                                    new() { Id = id.ToByteString() },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<FullToDoItem>> GetToDoItemAsync(
        Guid id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetToDoItemAsync(
                                    new() { Id = id.ToByteString() },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.ToFullToDoItem().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoShortItem>>
    > GetShortToDoItemsAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new GetShortToDoItemsRequest();
                            request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                            return client
                                .GetShortToDoItemsAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply =>
                                        reply
                                            .Items.Select(x => x.ToToDoShortItem())
                                            .ToArray()
                                            .ToReadOnlyMemory()
                                            .ToResult(),
                                    ct
                                );
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenToDoItemIdsAsync(
        OptionStruct<Guid> id,
        ReadOnlyMemory<Guid> ignoreIds,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new GetChildrenToDoItemIdsRequest()
                            {
                                Id = id.ToByteString()
                            };

                            request.IgnoreIds.AddRange(ignoreIds.ToByteString().ToArray());

                            return client
                                .GetChildrenToDoItemIdsAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct);
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetFavoriteToDoItemIdsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetFavoriteToDoItemIdsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> AddToDoItemAsync(
        ReadOnlyMemory<AddToDoItemOptions> options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new AddToDoItemRequest();

                            request.Items.AddRange(
                                options.Select(x => x.ToAddToDoItemOptionsGrpc()).ToArray()
                            );

                            return client
                                .AddToDoItemAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(id => id.Ids.ToGuid().ToResult(), ct);
                        },
                        ct
                    ),
            ct
        );
    }

    public Cvtar DeleteToDoItemsAsync(ReadOnlyMemory<Guid> ids, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new DeleteToDoItemRequest();
                            request.Ids.AddRange(ids.Select(x => x.ToByteString()).ToArray());

                            return client
                                .DeleteToDoItemAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultOnly()
                                .ConfigureAwait(false);
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetTodayToDoItemsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetTodayToDoItemsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(reply => reply.Ids.ToGuid().ToResult(), ct),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<ReadOnlyMemory<ToDoSelectorItem>>
    > GetToDoSelectorItemsAsync(ReadOnlyMemory<Guid> ignoreIds, CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new GetToDoSelectorItemsRequest();
                            request.IgnoreIds.AddRange(ignoreIds.ToByteString().ToArray());

                            return client
                                .GetToDoSelectorItemsAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Items.ToToDoSelectorItem().ToResult(),
                                    ct
                                );
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<string>> ToDoItemToStringAsync(
        ReadOnlyMemory<ToDoItemToStringOptions> options,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                        {
                            var request = new ToDoItemToStringRequest();

                            request.Items.AddRange(
                                options.Select(x => x.ToToDoItemToStringOptionsGrpc()).ToArray()
                            );

                            return client
                                .ToDoItemToStringAsync(
                                    request,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply =>
                                        reply
                                            .Value.ToResult()
                                            .ToValueTaskResult()
                                            .ConfigureAwait(false),
                                    ct
                                );
                        },
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<
        Result<OptionStruct<ActiveToDoItem>>
    > GetCurrentActiveToDoItemAsync(CancellationToken ct)
    {
        return CallClientAsync(
            client =>
                metadataFactory
                    .CreateAsync(ct)
                    .IfSuccessAsync(
                        metadata =>
                            client
                                .GetCurrentActiveToDoItemAsync(
                                    DefaultObject<GetCurrentActiveToDoItemRequest>.Default,
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Item.ToOptionActiveToDoItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            (client, token) =>
                GetToDoItemsCore(client, ids, chunkSize, token).ConfigureAwait(false),
            ct
        );
    }

    private async IAsyncEnumerable<Result<ReadOnlyMemory<ToDoItem>>> GetToDoItemsCore(
        ToDoService.ToDoServiceClient client,
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        if (ids.IsEmpty)
        {
            yield return ReadOnlyMemory<ToDoItem>.Empty.ToResult();

            yield break;
        }

        var request = new GetToDoItemsRequest { ChunkSize = chunkSize };

        var idsByteString = ids.ToByteString();
        request.Ids.AddRange(idsByteString.ToArray());
        var metadata = await metadataFactory.CreateAsync(ct);

        if (!metadata.TryGetValue(out var value))
        {
            yield return new(metadata.Errors);

            yield break;
        }

        using var response = client.GetToDoItems(request, value, DateTime.UtcNow.Add(Timeout), ct);

        while (await MoveNextAsync(response, ct))
        {
            var reply = response.ResponseStream.Current;
            var item = reply.Items.ToToDoItem().ToResult();

            yield return item;
        }
    }

    private async ValueTask<bool> MoveNextAsync<T>(
        AsyncServerStreamingCall<T> streamingCall,
        CancellationToken ct
    )
    {
        try
        {
            return await streamingCall.ResponseStream.MoveNext(ct);
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
            return false;
        }
    }
}
