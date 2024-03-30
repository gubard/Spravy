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
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcPasswordService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public Task<Result> AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = mapper.Map<AddPasswordItemRequest>(options);
                            cancellationToken.ThrowIfCancellationRequested();

                            await client.AddPasswordItemAsync(
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

    public Task<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetPasswordItemsRequest();
                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetPasswordItemsAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<ReadOnlyMemory<PasswordItem>>(reply.Items).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result<PasswordItem>> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GetPasswordItemRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            var reply = await client.GetPasswordItemAsync(
                                request,
                                value,
                                cancellationToken: cancellationToken
                            );

                            return mapper.Map<PasswordItem>(reply).ToResult();
                        }
                    );
            },
            cancellationToken
        );
    }

    public Task<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new DeletePasswordItemRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.DeletePasswordItemAsync(
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

    public Task<Result<string>> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new GeneratePasswordRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                            };

                            cancellationToken.ThrowIfCancellationRequested();

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
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdatePasswordItemNameRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Name = name,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemNameAsync(
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

    public Task<Result> UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdatePasswordItemKeyRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Key = key,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemKeyAsync(
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

    public Task<Result> UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdatePasswordItemLengthRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Length = length,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemLengthAsync(
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

    public Task<Result> UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            client =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return metadataFactory.CreateAsync(cancellationToken)
                    .IfSuccessAsync(
                        async value =>
                        {
                            var request = new UpdatePasswordItemRegexRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                Regex = regex,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemRegexAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
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
                            var request = new UpdatePasswordItemIsAvailableNumberRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                IsAvailableNumber = isAvailableNumber,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemIsAvailableNumberAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
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
                            var request = new UpdatePasswordItemIsAvailableLowerLatinRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                IsAvailableLowerLatin = isAvailableLowerLatin,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemIsAvailableLowerLatinAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
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
                            var request = new UpdatePasswordItemIsAvailableSpecialSymbolsRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
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

    public Task<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
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
                            var request = new UpdatePasswordItemCustomAvailableCharactersRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                CustomAvailableCharacters = customAvailableCharacters,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemCustomAvailableCharactersAsync(
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

    public Task<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
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
                            var request = new UpdatePasswordItemIsAvailableUpperLatinRequest
                            {
                                Id = mapper.Map<ByteString>(id),
                                IsAvailableUpperLatin = isAvailableUpperLatin,
                            };

                            cancellationToken.ThrowIfCancellationRequested();

                            await client.UpdatePasswordItemIsAvailableUpperLatinAsync(
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

    public static GrpcPasswordService CreateGrpcService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory, serializer);
    }
}