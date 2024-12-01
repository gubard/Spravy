namespace Spravy.PasswordGenerator.Service.Services;

[Authorize]
public class GrpcUserSecretService : UserSecretServiceBase
{
    private readonly ISerializer serializer;
    private readonly IUserSecretService userSecretService;

    public GrpcUserSecretService(IUserSecretService userSecretService, ISerializer serializer)
    {
        this.userSecretService = userSecretService;
        this.serializer = serializer;
    }

    public override Task<GetUserSecretReply> GetUserSecret(GetUserSecretRequest request, ServerCallContext context)
    {
        return userSecretService.GetUserSecretAsync(context.CancellationToken)
           .HandleAsync(
                serializer,
                userSecret => new GetUserSecretReply
                {
                    Secret = userSecret.ToByteString(),
                },
                context.CancellationToken
            );
    }
}