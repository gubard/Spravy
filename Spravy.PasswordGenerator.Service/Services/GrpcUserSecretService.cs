using AutoMapper;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Spravy.PasswordGenerator.Domain.Interfaces;
using Spravy.PasswordGenerator.Protos;
using static Spravy.PasswordGenerator.Protos.UserSecretService;

namespace Spravy.PasswordGenerator.Service.Services;

[Authorize]
public class GrpcUserSecretService : UserSecretServiceBase
{
    private readonly IUserSecretService userSecretService;
    private readonly IMapper mapper;

    public GrpcUserSecretService(IUserSecretService userSecretService, IMapper mapper)
    {
        this.userSecretService = userSecretService;
        this.mapper = mapper;
    }

    public override async Task<GetUserSecretReply> GetUserSecret(
        GetUserSecretRequest request,
        ServerCallContext context
    )
    {
        var userSecret = await userSecretService.GetUserSecretAsync(context.CancellationToken);

        var reply = new GetUserSecretReply
        {
            Secret = mapper.Map<ByteString>(userSecret),
        };

        return reply;
    }
}