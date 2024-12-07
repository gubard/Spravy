using Grpc.Core;
using Spravy.Core.Interfaces;
using Spravy.Core.Mappers;
using Spravy.PasswordGenerator.Domain.Mapper.Mappers;
using Spravy.PasswordGenerator.Protos;

namespace Spravy.PasswordGenerator.Domain.Client.Services;

public class GrpcPasswordService : GrpcServiceBase<PasswordServiceClient>,
    IPasswordService,
    IGrpcServiceCreatorAuth<GrpcPasswordService, PasswordServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    private readonly IMetadataFactory metadataFactory;

    public GrpcPasswordService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    ) : base(grpcClientFactory, host, handler, retryService)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcPasswordService CreateGrpcService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler,
        IRetryService retryService
    )
    {
        return new(
            grpcClientFactory,
            host,
            metadataFactory,
            handler,
            retryService
        );
    }

    public Cvtar AddPasswordItemAsync(AddPasswordOptions options, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.AddPasswordItemAsync(
                            options.ToAddPasswordItemRequest(),
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

    public Cvtar EditPasswordItemsAsync(EditPasswordItems options, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.EditPasswordItemsAsync(
                            new()
                            {
                                Value = options.ToEditPasswordItemsGrpc(),
                            },
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

    public Cvtar DeletePasswordItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.DeletePasswordItemAsync(
                            new()
                            {
                                Id = id.ToByteString(),
                            },
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

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.GeneratePasswordAsync(
                            new()
                            {
                                Id = id.ToByteString(),
                            },
                            metadata,
                            DateTime.UtcNow.Add(Timeout),
                            ct
                        )
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(
                            reply => reply.Password.ToResult().ToValueTaskResult().ConfigureAwait(false),
                            ct
                        ),
                    ct
                ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<Guid>>> GetChildrenPasswordItemIdsAsync(
        OptionStruct<Guid> id,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata =>
                    {
                        var request = new GetChildrenPasswordItemIdsRequest
                        {
                            Id = id.ToByteString(),
                        };

                        return client.GetChildrenPasswordItemIdsAsync(
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

    public ConfiguredCancelableAsyncEnumerable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        CancellationToken ct
    )
    {
        return CallClientAsync(
            (client, token) => GetPasswordItemsCore(client, ids, chunkSize, token).ConfigureAwait(false),
            ct
        );
    }

    private async IAsyncEnumerable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsCore(
        PasswordServiceClient client,
        ReadOnlyMemory<Guid> ids,
        uint chunkSize,
        [EnumeratorCancellation] CancellationToken ct
    )
    {
        if (ids.IsEmpty)
        {
            yield return ReadOnlyMemory<PasswordItem>.Empty.ToResult();

            yield break;
        }

        var request = new GetPasswordItemsRequest
        {
            ChunkSize = chunkSize,
        };

        var idsByteString = ids.ToByteString();
        request.Ids.AddRange(idsByteString.ToArray());
        var metadata = await metadataFactory.CreateAsync(ct);

        if (!metadata.TryGetValue(out var value))
        {
            yield return new(metadata.Errors);

            yield break;
        }

        using var response = client.GetPasswordItems(request, value, DateTime.UtcNow.Add(Timeout), ct);

        while (await MoveNextAsync(response, ct))
        {
            var reply = response.ResponseStream.Current;
            var item = reply.Items.ToPasswordItem().ToResult();

            yield return item;
        }
    }

    private async ValueTask<bool> MoveNextAsync<T>(AsyncServerStreamingCall<T> streamingCall, CancellationToken ct)
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