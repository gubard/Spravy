using System.Runtime.CompilerServices;
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
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);
    
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

    public ConfiguredValueTaskAwaitable<Result> AddPasswordItemAsync(
        AddPasswordOptions options,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => converter.Convert<AddPasswordItemRequest>(options)
               .IfSuccessAsync(
                    request => metadataFactory.CreateAsync(cancellationToken)
                       .IfSuccessAsync(
                            metadata => client.AddPasswordItemAsync(request, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
                               .ToValueTaskResultOnly()
                               .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<ReadOnlyMemory<PasswordItem>>> GetPasswordItemsAsync(
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(
            client => metadataFactory.CreateAsync(cancellationToken)
               .IfSuccessAsync(
                    metadata => client.GetPasswordItemsAsync(new(), metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
                       .ToValueTaskResultValueOnly()
                       .ConfigureAwait(false)
                       .IfSuccessAsync(
                            reply => converter.Convert<PasswordItem[]>(reply.Items)
                               .IfSuccess(items => items.ToReadOnlyMemory().ToResult())
                               .ToValueTaskResult()
                               .ConfigureAwait(false), cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<PasswordItem>> GetPasswordItemAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GetPasswordItemAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
               .ToValueTaskResultValueOnly()
               .ConfigureAwait(false)
               .IfSuccessAsync(
                    reply => converter.Convert<PasswordItem>(reply).ToValueTaskResult().ConfigureAwait(false),
                    cancellationToken), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result> DeletePasswordItemAsync(Guid id, CancellationToken cancellationToken)
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.DeletePasswordItemAsync(new()
                {
                    Id = i,
                }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
               .ToValueTaskResultOnly()
               .ConfigureAwait(false), cancellationToken), cancellationToken);
    }

    public ConfiguredValueTaskAwaitable<Result<string>> GeneratePasswordAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return CallClientAsync(client => metadataFactory.CreateAsync(cancellationToken)
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.GeneratePasswordAsync(new()
                    {
                        Id = i,
                    }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdatePasswordItemNameAsync(new()
                {
                    Id = i,
                    Name = name,
                }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdatePasswordItemKeyAsync(new()
                {
                    Id = i,
                    Key = key,
                }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdatePasswordItemLengthAsync(new()
                {
                    Id = i,
                    Length = length,
                }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) => client.UpdatePasswordItemRegexAsync(new()
                {
                    Id = i,
                    Regex = regex,
                }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) =>
                client.UpdatePasswordItemIsAvailableNumberAsync(
                        new()
                        {
                            Id = i,
                            IsAvailableNumber = isAvailableNumber,
                        }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) =>
                client.UpdatePasswordItemIsAvailableLowerLatinAsync(new()
                    {
                        Id = i,
                        IsAvailableLowerLatin = isAvailableLowerLatin,
                    }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) =>
                client.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(new()
                    {
                        Id = i,
                        IsAvailableSpecialSymbols = isAvailableSpecialSymbols,
                    }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) =>
                client.UpdatePasswordItemCustomAvailableCharactersAsync(new()
                    {
                        Id = i,
                        CustomAvailableCharacters = customAvailableCharacters,
                    }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
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
           .IfSuccessAsync(converter.Convert<ByteString>(id), (metadata, i) =>
                client.UpdatePasswordItemIsAvailableUpperLatinAsync(new()
                    {
                        Id = i,
                        IsAvailableUpperLatin = isAvailableUpperLatin,
                    }, metadata, DateTime.UtcNow.Add(Timeout),  cancellationToken)
                   .ToValueTaskResultOnly()
                   .ConfigureAwait(false), cancellationToken), cancellationToken);
    }
}