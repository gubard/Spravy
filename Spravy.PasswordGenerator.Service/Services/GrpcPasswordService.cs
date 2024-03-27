using AutoMapper;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Domain.Models;
using Spravy.PasswordGenerator.Protos;
using static Spravy.PasswordGenerator.Protos.PasswordService;

namespace Spravy.PasswordGenerator.Service.Services;

[Authorize]
public class GrpcPasswordService : PasswordServiceBase
{
    private readonly IPasswordService passwordService;
    private readonly IMapper mapper;

    public GrpcPasswordService(IPasswordService passwordService, IMapper mapper)
    {
        this.passwordService = passwordService;
        this.mapper = mapper;
    }

    public override async Task<GeneratePasswordReply> GeneratePassword(
        GeneratePasswordRequest request,
        ServerCallContext context
    )
    {
        var password = await passwordService.GeneratePasswordAsync(
            mapper.Map<Guid>(request.Id),
            context.CancellationToken
        );

        return new GeneratePasswordReply
        {
            Password = password
        };
    }

    public override async Task<GetPasswordItemReply> GetPasswordItem(
        GetPasswordItemRequest request,
        ServerCallContext context
    )
    {
        var item = await passwordService.GetPasswordItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return mapper.Map<GetPasswordItemReply>(item);
    }

    public override async Task<AddPasswordItemReply> AddPasswordItem(
        AddPasswordItemRequest request,
        ServerCallContext context
    )
    {
        await passwordService.AddPasswordItemAsync(mapper.Map<AddPasswordOptions>(request), context.CancellationToken);

        return new AddPasswordItemReply();
    }

    public override async Task<GetPasswordItemsReply> GetPasswordItems(
        GetPasswordItemsRequest request,
        ServerCallContext context
    )
    {
        var passwordItems = await passwordService.GetPasswordItemsAsync(context.CancellationToken);
        var reply = new GetPasswordItemsReply();
        reply.Items.AddRange(passwordItems.Select(item => mapper.Map<PasswordItemGrpc>(item)));

        return reply;
    }

    public override async Task<DeletePasswordItemReply> DeletePasswordItem(
        DeletePasswordItemRequest request,
        ServerCallContext context
    )
    {
        await passwordService.DeletePasswordItemAsync(mapper.Map<Guid>(request.Id), context.CancellationToken);

        return new DeletePasswordItemReply();
    }

    public override async Task<UpdatePasswordItemNameReply> UpdatePasswordItemName(
        UpdatePasswordItemNameRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemNameAsync(
            mapper.Map<Guid>(request.Id),
            request.Name,
            context.CancellationToken
        );

        return new UpdatePasswordItemNameReply();
    }

    public override async Task<UpdatePasswordItemKeyReply> UpdatePasswordItemKey(
        UpdatePasswordItemKeyRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemKeyAsync(
            mapper.Map<Guid>(request.Id),
            request.Key,
            context.CancellationToken
        );

        return new UpdatePasswordItemKeyReply();
    }

    public override async Task<UpdatePasswordItemLengthReply> UpdatePasswordItemLength(
        UpdatePasswordItemLengthRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemLengthAsync(
            mapper.Map<Guid>(request.Id),
            (ushort)request.Length,
            context.CancellationToken
        );

        return new UpdatePasswordItemLengthReply();
    }

    public override async Task<UpdatePasswordItemRegexReply> UpdatePasswordItemRegex(
        UpdatePasswordItemRegexRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemRegexAsync(
            mapper.Map<Guid>(request.Id),
            request.Regex,
            context.CancellationToken
        );

        return new UpdatePasswordItemRegexReply();
    }

    public override async Task<UpdatePasswordItemCustomAvailableCharactersReply>
        UpdatePasswordItemCustomAvailableCharacters(
            UpdatePasswordItemCustomAvailableCharactersRequest request,
            ServerCallContext context
        )
    {
        await passwordService.UpdatePasswordItemCustomAvailableCharactersAsync(
            mapper.Map<Guid>(request.Id),
            request.CustomAvailableCharacters,
            context.CancellationToken
        );

        return new UpdatePasswordItemCustomAvailableCharactersReply();
    }

    public override async Task<UpdatePasswordItemIsAvailableNumberReply> UpdatePasswordItemIsAvailableNumber(
        UpdatePasswordItemIsAvailableNumberRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemIsAvailableNumberAsync(
            mapper.Map<Guid>(request.Id),
            request.IsAvailableNumber,
            context.CancellationToken
        );

        return new UpdatePasswordItemIsAvailableNumberReply();
    }

    public override async Task<UpdatePasswordItemIsAvailableLowerLatinReply> UpdatePasswordItemIsAvailableLowerLatin(
        UpdatePasswordItemIsAvailableLowerLatinRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemIsAvailableLowerLatinAsync(
            mapper.Map<Guid>(request.Id),
            request.IsAvailableLowerLatin,
            context.CancellationToken
        );

        return new UpdatePasswordItemIsAvailableLowerLatinReply();
    }

    public override async Task<UpdatePasswordItemIsAvailableSpecialSymbolsReply>
        UpdatePasswordItemIsAvailableSpecialSymbols(
            UpdatePasswordItemIsAvailableSpecialSymbolsRequest request,
            ServerCallContext context
        )
    {
        await passwordService.UpdatePasswordItemIsAvailableSpecialSymbolsAsync(
            mapper.Map<Guid>(request.Id),
            request.IsAvailableSpecialSymbols,
            context.CancellationToken
        );

        return new UpdatePasswordItemIsAvailableSpecialSymbolsReply();
    }

    public override async Task<UpdatePasswordItemIsAvailableUpperLatinReply> UpdatePasswordItemIsAvailableUpperLatin(
        UpdatePasswordItemIsAvailableUpperLatinRequest request,
        ServerCallContext context
    )
    {
        await passwordService.UpdatePasswordItemIsAvailableUpperLatinAsync(
            mapper.Map<Guid>(request.Id),
            request.IsAvailableUpperLatin,
            context.CancellationToken
        );

        return new UpdatePasswordItemIsAvailableUpperLatinReply();
    }
}