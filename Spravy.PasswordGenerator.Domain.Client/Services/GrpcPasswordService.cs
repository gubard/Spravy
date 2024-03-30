using AutoMapper;
using Google.Protobuf;
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

    public Task<Result> AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<AddPasswordItemRequest>(options),
                        async (value, request) =>
                        {
                            await client.AddPasswordItemAsync(
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

    public Task<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetPasswordItemsRequest();

                            var reply = await client.GetPasswordItemsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<PasswordItem[]>(reply.Items)
                                .IfSuccess(items => items.ToReadOnlyMemory().ToResult());
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new GetPasswordItemRequest
                            {
                                Id = i,
                            };

                            var reply = await client.GetPasswordItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return converter.Convert<PasswordItem>(reply);
                        }
                    ),
            cancellationToken
        );
    }

    public Task<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new DeletePasswordItemRequest
                            {
                                Id = i,
                            };

                            await client.DeletePasswordItemAsync(
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

    public Task<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken)
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
                            var request = new GeneratePasswordRequest
                            {
                                Id = i,
                            };

                            var reply = await client.GeneratePasswordAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return reply.Password.ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdatePasswordItemNameRequest
                            {
                                Id = i,
                                Name = name,
                            };

                            await client.UpdatePasswordItemNameAsync(
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

    public Task<Result> UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdatePasswordItemKeyRequest
                            {
                                Id = i,
                                Key = key,
                            };

                            await client.UpdatePasswordItemKeyAsync(
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

    public Task<Result> UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdatePasswordItemLengthRequest
                            {
                                Id = i,
                                Length = length,
                            };

                            await client.UpdatePasswordItemLengthAsync(
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

    public Task<Result> UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
                metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        converter.Convert<ByteString>(id),
                        async (value, i) =>
                        {
                            var request = new UpdatePasswordItemRegexRequest
                            {
                                Id = i,
                                Regex = regex,
                            };

                            await client.UpdatePasswordItemRegexAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
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
                            var request = new UpdatePasswordItemIsAvailableNumberRequest
                            {
                                Id = i,
                                IsAvailableNumber = isAvailableNumber,
                            };

                            await client.UpdatePasswordItemIsAvailableNumberAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
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
                            var request = new UpdatePasswordItemIsAvailableLowerLatinRequest
                            {
                                Id = i,
                                IsAvailableLowerLatin = isAvailableLowerLatin,
                            };

                            await client.UpdatePasswordItemIsAvailableLowerLatinAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
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
                            var request = new UpdatePasswordItemIsAvailableSpecialSymbolsRequest
                            {
                                Id = i,
                                IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
                            };

                            await client.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
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

    public Task<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
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
                            var request = new UpdatePasswordItemCustomAvailableCharactersRequest
                            {
                                Id = i,
                                CustomAvailableCharacters = customAvailableCharacters,
                            };

                            await client.UpdatePasswordItemCustomAvailableCharactersAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
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
                            var request = new UpdatePasswordItemIsAvailableUpperLatinRequest
                            {
                                Id = i,
                                IsAvailableUpperLatin = isAvailableUpperLatin,
                            };

                            await client.UpdatePasswordItemIsAvailableUpperLatinAsync(
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