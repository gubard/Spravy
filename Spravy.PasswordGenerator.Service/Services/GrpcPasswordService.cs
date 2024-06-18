using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Core.Mappers;
using Spravy.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Mapper.Mappers;
using Spravy.PasswordGenerator.Protos;
using Spravy.Service.Extensions;
using static Spravy.PasswordGenerator.Protos.PasswordService;

namespace Spravy.PasswordGenerator.Service.Services;

[Authorize]
public class GrpcPasswordService : PasswordServiceBase
{
    private readonly IPasswordService passwordService;
    private readonly ISerializer serializer;

    public GrpcPasswordService(IPasswordService passwordService, ISerializer serializer)
    {
        this.passwordService = passwordService;
        this.serializer = serializer;
    }

    public override Task<GeneratePasswordReply> GeneratePassword(
        GeneratePasswordRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GeneratePasswordAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(serializer, value => new GeneratePasswordReply
            {
                Password = value,
            });
    }

    public override Task<GetPasswordItemReply> GetPasswordItem(
        GetPasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GetPasswordItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync(serializer, value => value.ToGetPasswordItemReply());
    }

    public override Task<AddPasswordItemReply> AddPasswordItem(
        AddPasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.AddPasswordItemAsync(request.ToAddPasswordOptions(), context.CancellationToken)
           .HandleAsync<AddPasswordItemReply>(serializer);
    }

    public override Task<GetPasswordItemsReply> GetPasswordItems(
        GetPasswordItemsRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GetPasswordItemsAsync(context.CancellationToken)
           .HandleAsync(serializer, value =>
            {
                var reply = new GetPasswordItemsReply();
                reply.Items.AddRange(value.ToPasswordItemGrpc().ToArray());

                return reply;
            });
    }

    public override Task<DeletePasswordItemReply> DeletePasswordItem(
        DeletePasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.DeletePasswordItemAsync(request.Id.ToGuid(), context.CancellationToken)
           .HandleAsync<DeletePasswordItemReply>(serializer);
    }

    public override Task<UpdatePasswordItemNameReply> UpdatePasswordItemName(
        UpdatePasswordItemNameRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemNameAsync(request.Id.ToGuid(), request.Name, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemNameReply>(serializer);
    }

    public override Task<UpdatePasswordItemKeyReply> UpdatePasswordItemKey(
        UpdatePasswordItemKeyRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemKeyAsync(request.Id.ToGuid(), request.Key, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemKeyReply>(serializer);
    }

    public override Task<UpdatePasswordItemLengthReply> UpdatePasswordItemLength(
        UpdatePasswordItemLengthRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemLengthAsync(request.Id.ToGuid(), (ushort)request.Length,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemLengthReply>(serializer);
    }

    public override Task<UpdatePasswordItemRegexReply> UpdatePasswordItemRegex(
        UpdatePasswordItemRegexRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemRegexAsync(request.Id.ToGuid(), request.Regex, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemRegexReply>(serializer);
    }

    public override Task<UpdatePasswordItemCustomAvailableCharactersReply> UpdatePasswordItemCustomAvailableCharacters(
        UpdatePasswordItemCustomAvailableCharactersRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemCustomAvailableCharactersAsync(request.Id.ToGuid(),
                request.CustomAvailableCharacters, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemCustomAvailableCharactersReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableNumberReply> UpdatePasswordItemIsAvailableNumber(
        UpdatePasswordItemIsAvailableNumberRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableNumberAsync(request.Id.ToGuid(), request.IsAvailableNumber,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableNumberReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableLowerLatinReply> UpdatePasswordItemIsAvailableLowerLatin(
        UpdatePasswordItemIsAvailableLowerLatinRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableLowerLatinAsync(request.Id.ToGuid(), request.IsAvailableLowerLatin,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableLowerLatinReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableSpecialSymbolsReply> UpdatePasswordItemIsAvailableSpecialSymbols(
        UpdatePasswordItemIsAvailableSpecialSymbolsRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableSpecialSymbolsAsync(request.Id.ToGuid(),
                request.IsAvailableSpecialSymbols, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableSpecialSymbolsReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableUpperLatinReply> UpdatePasswordItemIsAvailableUpperLatin(
        UpdatePasswordItemIsAvailableUpperLatinRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableUpperLatinAsync(request.Id.ToGuid(), request.IsAvailableUpperLatin,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableUpperLatinReply>(serializer);
    }
}