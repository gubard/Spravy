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
}