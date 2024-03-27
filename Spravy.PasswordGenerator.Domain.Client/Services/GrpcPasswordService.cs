using AutoMapper;
using Google.Protobuf;
using Spravy.Client.Interfaces;
using Spravy.Client.Services;
using Spravy.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;
using static Spravy.PasswordGenerator.Protos.PasswordService;

namespace Spravy.PasswordGenerator.Domain.Client.Services;

public class GrpcPasswordService : GrpcServiceBase<PasswordServiceClient>,
    IPasswordService,
    IGrpcServiceCreator<GrpcPasswordService, PasswordServiceClient>
{
    private readonly IMapper mapper;
    private readonly IMetadataFactory metadataFactory;

    public GrpcPasswordService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    ) : base(grpcClientFactory, host)
    {
        this.mapper = mapper;
        this.metadataFactory = metadataFactory;
    }

    public Task AddPasswordItemAsync(AddPasswordOptions options, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = mapper.Map<AddPasswordItemRequest>(options);
                cancellationToken.ThrowIfCancellationRequested();

                await client.AddPasswordItemAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<IEnumerable<PasswordItem>> GetPasswordItemsAsync(CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetPasswordItemsRequest();
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetPasswordItemsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<IEnumerable<PasswordItem>>(reply.Items);
            },
            cancellationToken
        );
    }

    public Task<PasswordItem> GetPasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);
                var request = new GetPasswordItemRequest();
                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GetPasswordItemAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return mapper.Map<PasswordItem>(reply);
            },
            cancellationToken
        );
    }

    public Task DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new DeletePasswordItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.DeletePasswordItemAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task<string> GeneratePasswordAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new GeneratePasswordRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                var reply = await client.GeneratePasswordAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );

                return reply.Password;
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemNameRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Name = name,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemNameAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemKeyAsync(Guid id, string key, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemKeyRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Key = key,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemKeyAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemLengthAsync(Guid id, ushort length, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemLengthRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Length = length,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemLengthAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemRegexAsync(Guid id, string regex, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemRegexRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    Regex = regex,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemRegexAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemIsAvailableNumberRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsAvailableNumber = isAvailableNumber,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemIsAvailableNumberAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemIsAvailableLowerLatinRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsAvailableLowerLatin = isAvailableLowerLatin,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemIsAvailableLowerLatinAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemIsAvailableSpecialSymbolsRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemCustomAvailableCharactersRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    CustomAvailableCharacters = customAvailableCharacters,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemCustomAvailableCharactersAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public Task UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new UpdatePasswordItemIsAvailableUpperLatinRequest
                {
                    Id = mapper.Map<ByteString>(id),
                    IsAvailableUpperLatin = isAvailableUpperLatin,
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.UpdatePasswordItemIsAvailableUpperLatinAsync(
                    request,
                    metadata,
                    cancellationToken: cancellationToken
                );
            },
            cancellationToken
        );
    }

    public static GrpcPasswordService CreateGrpcService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMapper mapper,
        IMetadataFactory metadataFactory
    )
    {
        return new(grpcClientFactory, host, mapper, metadataFactory);
    }
}