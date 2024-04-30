using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;
using Spravy.Service.Extensions;
using static Spravy.PasswordGenerator.Protos.PasswordService;

namespace Spravy.PasswordGenerator.Service.Services;

[Authorize]
public class GrpcPasswordService : PasswordServiceBase
{
    private readonly IMapper mapper;
    private readonly IPasswordService passwordService;
    private readonly ISerializer serializer;

    public GrpcPasswordService(IPasswordService passwordService, IMapper mapper, ISerializer serializer)
    {
        this.passwordService = passwordService;
        this.mapper = mapper;
        this.serializer = serializer;
    }

    public override Task<GeneratePasswordReply> GeneratePassword(
        GeneratePasswordRequest request,
        ServerCallContext context
    )
    {
        return passwordService.GeneratePasswordAsync(mapper.Map<Guid>(request.Id), context.CancellationToken)
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
        return passwordService.GetPasswordItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken)
           .HandleAsync(serializer, value => mapper.Map<GetPasswordItemReply>(value));
    }

    public override Task<AddPasswordItemReply> AddPasswordItem(
        AddPasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.AddPasswordItemAsync(mapper.Map<AddPasswordOptions>(request), context.CancellationToken)
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
                reply.Items.AddRange(mapper.Map<PasswordItemGrpc[]>(value.ToArray()));

                return reply;
            });
    }

    public override Task<DeletePasswordItemReply> DeletePasswordItem(
        DeletePasswordItemRequest request,
        ServerCallContext context
    )
    {
        return passwordService.DeletePasswordItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken)
           .HandleAsync<DeletePasswordItemReply>(serializer);
    }

    public override Task<UpdatePasswordItemNameReply> UpdatePasswordItemName(
        UpdatePasswordItemNameRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemNameAsync(mapper.Map<Guid>(request.Id), request.Name, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemNameReply>(serializer);
    }

    public override Task<UpdatePasswordItemKeyReply> UpdatePasswordItemKey(
        UpdatePasswordItemKeyRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemKeyAsync(mapper.Map<Guid>(request.Id), request.Key, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemKeyReply>(serializer);
    }

    public override Task<UpdatePasswordItemLengthReply> UpdatePasswordItemLength(
        UpdatePasswordItemLengthRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemLengthAsync(mapper.Map<Guid>(request.Id), (ushort)request.Length,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemLengthReply>(serializer);
    }

    public override Task<UpdatePasswordItemRegexReply> UpdatePasswordItemRegex(
        UpdatePasswordItemRegexRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemRegexAsync(mapper.Map<Guid>(request.Id), request.Regex, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemRegexReply>(serializer);
    }

    public override Task<UpdatePasswordItemCustomAvailableCharactersReply> UpdatePasswordItemCustomAvailableCharacters(
        UpdatePasswordItemCustomAvailableCharactersRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemCustomAvailableCharactersAsync(mapper.Map<Guid>(request.Id),
                request.CustomAvailableCharacters, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemCustomAvailableCharactersReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableNumberReply> UpdatePasswordItemIsAvailableNumber(
        UpdatePasswordItemIsAvailableNumberRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableNumberAsync(mapper.Map<Guid>(request.Id), request.IsAvailableNumber,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableNumberReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableLowerLatinReply> UpdatePasswordItemIsAvailableLowerLatin(
        UpdatePasswordItemIsAvailableLowerLatinRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableLowerLatinAsync(mapper.Map<Guid>(request.Id), request.IsAvailableLowerLatin,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableLowerLatinReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableSpecialSymbolsReply> UpdatePasswordItemIsAvailableSpecialSymbols(
        UpdatePasswordItemIsAvailableSpecialSymbolsRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableSpecialSymbolsAsync(mapper.Map<Guid>(request.Id),
                request.IsAvailableSpecialSymbols, context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableSpecialSymbolsReply>(serializer);
    }

    public override Task<UpdatePasswordItemIsAvailableUpperLatinReply> UpdatePasswordItemIsAvailableUpperLatin(
        UpdatePasswordItemIsAvailableUpperLatinRequest request,
        ServerCallContext context
    )
    {
        return passwordService
           .UpdatePasswordItemIsAvailableUpperLatinAsync(mapper.Map<Guid>(request.Id), request.IsAvailableUpperLatin,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordItemIsAvailableUpperLatinReply>(serializer);
    }
}