using Grpc.Core;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Mapper.Mappers;
using Spravy.Authentication.Protos;
using Spravy.Domain.Interfaces;
using Spravy.Service.Extensions;

namespace Spravy.Authentication.Service.Services;

public class GrpcAuthenticationService : AuthenticationService.AuthenticationServiceBase
{
    private readonly IAuthenticationService authenticationService;
    private readonly ISerializer serializer;

    public GrpcAuthenticationService(IAuthenticationService authenticationService, ISerializer serializer)
    {
        this.authenticationService = authenticationService;
        this.serializer = serializer;
    }

    public override Task<UpdatePasswordByLoginReply> UpdatePasswordByLogin(
        UpdatePasswordByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdatePasswordByLoginAsync(request.Login, request.VerificationCode, request.NewPassword,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordByLoginReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdatePasswordByEmailReply> UpdatePasswordByEmail(
        UpdatePasswordByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdatePasswordByEmailAsync(request.Email, request.VerificationCode, request.NewPassword,
                context.CancellationToken)
           .HandleAsync<UpdatePasswordByEmailReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateEmailNotVerifiedUserByLoginReply> UpdateEmailNotVerifiedUserByLogin(
        UpdateEmailNotVerifiedUserByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdateEmailNotVerifiedUserByLoginAsync(request.Login, request.NewEmail, context.CancellationToken)
           .HandleAsync<UpdateEmailNotVerifiedUserByLoginReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateEmailNotVerifiedUserByEmailReply> UpdateEmailNotVerifiedUserByEmail(
        UpdateEmailNotVerifiedUserByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .UpdateEmailNotVerifiedUserByEmailAsync(request.Email, request.NewEmail, context.CancellationToken)
           .HandleAsync<UpdateEmailNotVerifiedUserByEmailReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateVerificationCodeByLoginReply> UpdateVerificationCodeByLogin(
        UpdateVerificationCodeByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.UpdateVerificationCodeByLoginAsync(request.Login, context.CancellationToken)
           .HandleAsync<UpdateVerificationCodeByLoginReply>(serializer, context.CancellationToken);
    }

    public override Task<UpdateVerificationCodeByEmailReply> UpdateVerificationCodeByEmail(
        UpdateVerificationCodeByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService.UpdateVerificationCodeByEmailAsync(request.Email, context.CancellationToken)
           .HandleAsync<UpdateVerificationCodeByEmailReply>(serializer, context.CancellationToken);
    }

    public override Task<VerifiedEmailByLoginReply> VerifiedEmailByLogin(
        VerifiedEmailByLoginRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .VerifiedEmailByLoginAsync(request.Login, request.VerificationCode, context.CancellationToken)
           .HandleAsync<VerifiedEmailByLoginReply>(serializer, context.CancellationToken);
    }

    public override Task<VerifiedEmailByEmailReply> VerifiedEmailByEmail(
        VerifiedEmailByEmailRequest request,
        ServerCallContext context
    )
    {
        return authenticationService
           .VerifiedEmailByEmailAsync(request.Email, request.VerificationCode, context.CancellationToken)
           .HandleAsync<VerifiedEmailByEmailReply>(serializer, context.CancellationToken);
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
            }, context.CancellationToken);
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
            }, context.CancellationToken);
    }

    public override Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        var options = request.ToCreateUserOptions();

        return authenticationService.CreateUserAsync(options, context.CancellationToken)
           .HandleAsync<CreateUserReply>(serializer, context.CancellationToken);
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var user = request.User.ToUser();

        return authenticationService.LoginAsync(user, context.CancellationToken)
           .HandleAsync(serializer, value => value.ToLoginReply(), context.CancellationToken);
    }

    public override Task<RefreshTokenReply> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        return authenticationService.RefreshTokenAsync(request.RefreshToken, context.CancellationToken)
           .HandleAsync(serializer, value => value.ToRefreshTokenReply(), context.CancellationToken);
    }
}