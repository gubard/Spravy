using Spravy.Core.Interfaces;
using Spravy.Core.Mappers;
using Spravy.PasswordGenerator.Domain.Mapper.Mappers;

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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken ct
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.GetPasswordItemsAsync(new(), metadata, DateTime.UtcNow.Add(Timeout), ct)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Items.ToPasswordItem().ToResult(), ct),
                    ct
                ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken ct)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(ct)
               .IfSuccessAsync(
                    metadata => client.GetPasswordItemAsync(
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
                       .IfSuccessAsync(reply => reply.ToPasswordItem().ToResult(), ct),
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
}