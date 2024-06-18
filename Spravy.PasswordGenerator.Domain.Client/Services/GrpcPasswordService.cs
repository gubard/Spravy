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
        ISerializer serializer
    ) : base(grpcClientFactory, host, serializer)
    {
        this.metadataFactory = metadataFactory;
    }

    public static GrpcPasswordService CreateGrpcService(
        IFactory<Uri, PasswordServiceClient> grpcClientFactory,
        Uri host,
        IMetadataFactory metadataFactory,
        ISerializer serializer
    )
    {
        return new(grpcClientFactory, host, metadataFactory, serializer);
    }

    public ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(
        AddPasswordOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .AddPasswordItemAsync(options.ToAddPasswordItemRequest(), metadata,
                            DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultOnly()
                       .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client
                       .GetPasswordItemsAsync(new(), metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(reply => reply.Items.ToPasswordItem().ToResult(), cancellationToken),
                    cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(metadata => client.GetPasswordItemAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.ToPasswordItem().ToResult(), cancellationToken),
                cancellationToken),
            cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.DeletePasswordItemAsync(new()
                {
                    Id = id.ToByteString(),
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.GeneratePasswordAsync(new()
                    {
                        Id = id.ToByteString(),
                    }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
                   .ToValueTaskResultValueOnly()
                   .ConfigureAwait(false)
                   .IfSuccessAsync(reply => reply.Password.ToResult().ToValueTaskResult().ConfigureAwait(false),
                        cancellationToken),
                cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemNameAsync(
        Guid id,
        string name,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemNameAsync(new()
                {
                    Id = id.ToByteString(),
                    Name = name,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemKeyAsync(
        Guid id,
        string key,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemKeyAsync(new()
                {
                    Id = id.ToByteString(),
                    Key = key,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemLengthAsync(
        Guid id,
        ushort length,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemLengthAsync(new()
                {
                    Id = id.ToByteString(),
                    Length = length,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemRegexAsync(
        Guid id,
        string regex,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemRegexAsync(new()
                {
                    Id = id.ToByteString(),
                    Regex = regex,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableNumberAsync(
        Guid id,
        bool isAvailableNumber,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemIsAvailableNumberAsync(new()
                {
                    Id = id.ToByteString(),
                    IsAvailableNumber = isAvailableNumber,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableLowerLatinAsync(
        Guid id,
        bool isAvailableLowerLatin,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemIsAvailableLowerLatinAsync(new()
                {
                    Id = id.ToByteString(),
                    IsAvailableLowerLatin = isAvailableLowerLatin,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
        Guid id,
        bool isAvailableSpecialSymbols,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(new()
                {
                    Id = id.ToByteString(),
                    IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemCustomAvailableCharactersAsync(
        Guid id,
        string customAvailableCharacters,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemCustomAvailableCharactersAsync(new()
                {
                    Id = id.ToByteString(),
                    CustomAvailableCharacters = customAvailableCharacters,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> UpdatePasswordItemIsAvailableUpperLatinAsync(
        Guid id,
        bool isAvailableUpperLatin,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(metadata => client.UpdatePasswordItemIsAvailableUpperLatinAsync(new()
                {
                    Id = id.ToByteString(),
                    IsAvailableUpperLatin = isAvailableUpperLatin,
                }, metadata, DateTime.UtcNow.Add(Timeout), cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }
}