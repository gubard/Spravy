namespace Spravy.PasswordGenerator.Domain.Client.Models;

public class GrpcPasswordServiceOptions : IGrpcOptionsValue
{
    public static string Section
    {
        get => "GrpcPasswordService";
    }

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}

public class GrpcPasswordServiceOptionsConfiguration
{
    public GrpcPasswordServiceOptions? GrpcPasswordService { get; set; }
}
