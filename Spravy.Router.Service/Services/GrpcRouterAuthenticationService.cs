using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Mappers;
using Spravy.Authentication.Protos;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;

namespace Spravy.Router.Service.Services;

public class GrpcRouterAuthenticationService : AuthenticationService.AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly ISerializer serializer;

    public GrpcRouterAuthenticationService(IAuthenticationService authenticationService, ISerializer serializer)
    {
        this.authenticationService = authenticationService;
        this.serializer = serializer;
    }

    public override async Task<UpdateEmailNotVerifiedUserByLoginReply> UpdateEmailNotVerifiedUserByLogin(
        UpdateEmailNotVerifiedUserByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateEmailNotVerifiedUserByLoginAsync(request.Login, request.NewEmail,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateEmailNotVerifiedUserByEmailReply> UpdateEmailNotVerifiedUserByEmail(
        UpdateEmailNotVerifiedUserByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateEmailNotVerifiedUserByEmailAsync(request.Email, request.NewEmail,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdateVerificationCodeByLoginReply> UpdateVerificationCodeByLogin(
        UpdateVerificationCodeByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateVerificationCodeByLoginAsync(request.Login, context.CancellationToken);

        return new();
    }

    public override async Task<UpdateVerificationCodeByEmailReply> UpdateVerificationCodeByEmail(
        UpdateVerificationCodeByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdateVerificationCodeByEmailAsync(request.Email, context.CancellationToken);

        return new();
    }

    public override async Task<VerifiedEmailByLoginReply> VerifiedEmailByLogin(
        VerifiedEmailByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.VerifiedEmailByLoginAsync(request.Login, request.VerificationCode,
            context.CancellationToken);

        return new();
    }

    public override async Task<VerifiedEmailByEmailReply> VerifiedEmailByEmail(
        VerifiedEmailByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.VerifiedEmailByEmailAsync(request.Email, request.VerificationCode,
            context.CancellationToken);

        return new();
    }

    public override async Task<UpdatePasswordByLoginReply> UpdatePasswordByLogin(
        UpdatePasswordByLoginRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdatePasswordByLoginAsync(request.Login, request.VerificationCode,
            request.NewPassword, context.CancellationToken);

        return new();
    }

    public override async Task<UpdatePasswordByEmailReply> UpdatePasswordByEmail(
        UpdatePasswordByEmailRequest request,
        ServerCallContext context
    )
    {
        await authenticationService.UpdatePasswordByEmailAsync(request.Email, request.VerificationCode,
            request.NewPassword, context.CancellationToken);

        return new();
    }

    public override Task<IsVerifiedByLoginReply> IsVerifiedByLogin(
        IsVerifiedByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.IsVerifiedByLoginAsync(request.Login, context.CancellationToken)
           .HandleAsync(serializer, value => new IsVerifiedByLoginReply
            {
                IsVerified = value,
            });
    }

    public override Task<IsVerifiedByEmailReply> IsVerifiedByEmail(
        IsVerifiedByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.IsVerifiedByEmailAsync(request.Email, context.CancellationToken)
           .HandleAsync(serializer, value => new IsVerifiedByEmailReply
            {
                IsVerified = value,
            });
    }

    public override async Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = request.ToCreateUserOptions();
        await authenticationService.CreateUserAsync(options, context.CancellationToken);

        return new();
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = request.User.ToUser();

        return authenticationService.LoginAsync(user, context.CancellationToken)
           .HandleAsync(serializer, token => token.ToLoginReply());
    }

    public override Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        return authenticationService.RefreshTokenAsync(request.RefreshToken, context.CancellationToken)
           .HandleAsync(serializer, login => login.ToRefreshTokenReply());
    }
}