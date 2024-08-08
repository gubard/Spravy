using Spravy.Client.Helpers;
using Spravy.Core.Interfaces;
using Spravy.Core.Mappers;
using Spravy.PasswordGenerator.Domain.Mapper.Mappers;

namespace Spravy.PasswordGenerator.Domain.Client.Services;

public class GrpcPasswordService
    : GrpcServiceBase<PasswordServiceClient>,
        IPasswordService,
        IGrpcServiceCreatorAuth<GrpcPasswordService, PasswordServiceClient>
{
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    private readonly IMetadataFactory metadataFactory;

    public GrpcPasswordService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    )
        : base(grpcClientFactory, host, handler)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcPasswordService CreateGrpcService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        IRpcExceptionHandler handler
    )
    {
        return new(grpcClientFactory, host, metadataFactory, handler);
    }

    public ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(
        AddPasswordOptions options,
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
                                .AddPasswordItemAsync(
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

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
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
                                .GetPasswordItemsAsync(
                                    new(),
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => reply.Items.ToPasswordItem().ToResult(),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
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
                                .GetPasswordItemAsync(
                                    new() { Id = id.ToByteString(), },
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

    public ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(
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
                                .DeletePasswordItemAsync(
                                    new() { Id = id.ToByteString(), },
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

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(
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
                                .GeneratePasswordAsync(
                                    new() { Id = id.ToByteString(), },
                                    metadata,
                                    DateTime.UtcNow.Add(Timeout),
                                    ct
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply =>
                                        reply
                                            .Password.ToResult()
                                            .ToValueTaskResult()
                                            .ConfigureAwait(false),
                                    ct
                                ),
                        ct
                    ),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemNameAsync(
        Guid id,
        string name,
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
                                .UpdatePasswordItemNameAsync(
                                    new() { Id = id.ToByteString(), Name = name, },
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemKeyAsync(
        Guid id,
        string key,
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
                                .UpdatePasswordItemKeyAsync(
                                    new() { Id = id.ToByteString(), Key = key, },
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLengthAsync(
        Guid id,
        ushort length,
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
                                .UpdatePasswordItemLengthAsync(
                                    new() { Id = id.ToByteString(), Length = length, },
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemRegexAsync(
        Guid id,
        string regex,
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
                                .UpdatePasswordItemRegexAsync(
                                    new() { Id = id.ToByteString(), Regex = regex, },
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
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
                                .UpdatePasswordItemIsAvailableNumberAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        IsAvailableNumber = isAvailableNumber,
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
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
                                .UpdatePasswordItemIsAvailableLowerLatinAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        IsAvailableLowerLatin = isAvailableLowerLatin,
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
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
                                .UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
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
                                .UpdatePasswordItemCustomAvailableCharactersAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        CustomAvailableCharacters = customAvailableCharacters,
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

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
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
                                .UpdatePasswordItemIsAvailableUpperLatinAsync(
                                    new()
                                    {
                                        Id = id.ToByteString(),
                                        IsAvailableUpperLatin = isAvailableUpperLatin,
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
}
