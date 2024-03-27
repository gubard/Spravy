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

    public Task RemovePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(
            async client =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var metadata = await metadataFactory.CreateAsync(cancellationToken);

                var request = new RemovePasswordItemRequest
                {
                    Id = mapper.Map<ByteString>(id),
                };

                cancellationToken.ThrowIfCancellationRequested();

                await client.RemovePasswordItemAsync(
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

                var request = new GeneratePasswordRequest()
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