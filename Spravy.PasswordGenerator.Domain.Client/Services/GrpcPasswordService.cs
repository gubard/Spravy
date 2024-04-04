using Google.Protobuf;
using Spravy.Client.Extensions;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;
using static Spravy.PasswordGenerator.Protos.PasswordService;

namespace Spravy.PasswordGenerator.Domain.Client.Services;

public class GrpcPasswordService : GrpcServiceBase<PasswordServiceClient>,
    IPasswordService,
    IGrpcServiceCreatorAuth<GrpcPasswordService, PasswordServiceClient>
{
    private readonly IConverter converter;
    private readonly IMetadataFactory metadataFactory;

    public GrpcPasswordService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.converter = converter;
        this.metadataFactory = metadataFactory;
    }

    public ValueTask<Result> AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                converter.Convert<AddPasswordItemRequest>(options)
                    .IfSuccessAsync(
                        request => metadataFactory.CreateAsync(cancellationToken)
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                value => client.AddPasswordItemAsync(
                                        request,
                                        value,
                                        cancellationToken: cancellationToken
                                    )
                                    .ToValueTaskResultOnly()
                                    .ConfigureAwait(false)
                            )
                            .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        value => client.GetPasswordItemsAsync(
                                new GetPasswordItemsRequest(),
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => converter.Convert<PasswordItem[]>(reply.Items)
                                    .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                                    .ToValueTaskResult()
                                    .ConfigureAwait(false)
                            )
                            .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.GetPasswordItemAsync(
                                    new GetPasswordItemRequest
                                    {
                                        Id = i,
                                    },
                                    value,
                                    cancellationToken: cancellationToken
                                )
                                .ToValueTaskResultValueOnly()
                                .ConfigureAwait(false)
                                .IfSuccessAsync(
                                    reply => converter.Convert<PasswordItem>(reply)
                                        .ToValueTaskResult()
                                        .ConfigureAwait(false)
                                )
                                .ConfigureAwait(false)
                    )
                    .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.DeletePasswordItemAsync(
                                    new DeletePasswordItemRequest
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

    public ValueTask<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
                .ConfigureAwait(false)
                .IfSuccessAsync(
                    converter.Convert<ByteString>(id),
                    (value, i) =>
                        client.GeneratePasswordAsync(
                                new GeneratePasswordRequest
                                {
                                    Id = i,
                                },
                                value,
                                cancellationToken: cancellationToken
                            )
                            .ToValueTaskResultValueOnly()
                            .ConfigureAwait(false)
                            .IfSuccessAsync(
                                reply => reply.Password.ToResult().ToValueTaskResult().ConfigureAwait(false)
                            )
                            .ConfigureAwait(false)
                )
                .ConfigureAwait(false),
            cancellationToken
        );
    }

    public ValueTask<Result> UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdatePasswordItemNameAsync(
                                    new UpdatePasswordItemNameRequest
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

    public ValueTask<Result> UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemKeyAsync(
                                new UpdatePasswordItemKeyRequest
                                {
                                    Id = i,
                                    Key = key,
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

    public ValueTask<Result> UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemLengthAsync(
                                new UpdatePasswordItemLengthRequest
                                {
                                    Id = i,
                                    Length = length,
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

    public ValueTask<Result> UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) =>
                            client.UpdatePasswordItemRegexAsync(
                                    new UpdatePasswordItemRegexRequest
                                    {
                                        Id = i,
                                        Regex = regex,
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

    public ValueTask<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemIsAvailableNumberAsync(
                                new UpdatePasswordItemIsAvailableNumberRequest
                                {
                                    Id = i,
                                    IsAvailableNumber = isAvailableNumber,
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

    public ValueTask<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemIsAvailableLowerLatinAsync(
                                new UpdatePasswordItemIsAvailableLowerLatinRequest
                                {
                                    Id = i,
                                    IsAvailableLowerLatin = isAvailableLowerLatin,
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

    public ValueTask<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
                                new UpdatePasswordItemIsAvailableSpecialSymbolsRequest
                                {
                                    Id = i,
                                    IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
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

    public ValueTask<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemCustomAvailableCharactersAsync(
                                new UpdatePasswordItemCustomAvailableCharactersRequest
                                {
                                    Id = i,
                                    CustomAvailableCharacters = customAvailableCharacters,
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

    public ValueTask<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .ConfigureAwait(false)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        (value, i) => client.UpdatePasswordItemIsAvailableUpperLatinAsync(
                                new UpdatePasswordItemIsAvailableUpperLatinRequest
                                {
                                    Id = i,
                                    IsAvailableUpperLatin = isAvailableUpperLatin,
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

    public static GrpcPasswordService CreateGrpcService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IConverter converter,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, converter, metadataFactory, serializer);
    }
}