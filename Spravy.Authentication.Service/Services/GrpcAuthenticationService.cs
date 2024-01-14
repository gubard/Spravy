using AutoMapper;
using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Authentication.Protos;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using static Spravy.Authentication.Protos.AuthenticationService;

namespace Spravy.Authentication.Service.Services;

public class GrpcAuthenticationService : AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly IMapper mapper;
    private readonly IEmailService emailService;
    private readonly IRandom<string> randomString;

    public GrpcAuthenticationService(
        IAuthenticationService authenticationService,
        IMapper mapper,
        IEmailService emailService,
        IRandom<string> randomString
    )
    {
        this.authenticationService = authenticationService;
        this.mapper = mapper;
        this.emailService = emailService;
        this.randomString = randomString;
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = mapper.Map<CreateUserOptions>(request);
        await authenticationService.CreateUserAsync(options, context.CancellationToken);

        await emailService.SendEmailAsync(
            "Verification code",
            options.Email,
            randomString.GetRandom().ThrowIfNull(),
            CancellationToken.None
        );

        return new CreateUserReply();
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = mapper.Map<User>(request.User);
        var result = await authenticationService.LoginAsync(user, context.CancellationToken);
        var reply = mapper.Map<LoginReply>(result);

        return reply;
    }

    public override async Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        var result = await authenticationService.RefreshTokenAsync(request.RefreshToken, context.CancellationToken);
        var reply = mapper.Map<RefreshTokenReply>(result);

        return reply;
    }
}