using AutoMapper;
using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Service.Services;

public class GrpcAuthenticationService : AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly IMapper mapper;

    public GrpcAuthenticationService(
        IAuthenticationService authenticationService,
        IMapper mapper
    )
    {
        this.authenticationService = authenticationService;
        this.mapper = mapper;
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);
        await authenticationService.CreateUserAsync(options);

        return new CreateUserReply();
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);
        var result = await authenticationService.LoginAsync(user);
        var reply = mapper.Map<LoginReply>(result);

        return reply;
    }

    public override async Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await authenticationService.RefreshTokenAsync(request.RefreshToken);
        var reply = mapper.Map<RefreshTokenReply>(result);

        return reply;
    }
}