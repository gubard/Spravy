namespace Spravy.Authentication.Domain.Client.Models;

public class GrpcAuthenticationServiceOptions : IGrpcOptionsValue
{
    public static string Section => "GrpcAuthenticationService";

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}

public class GrpcAuthenticationServiceOptionsConfiguration
{
    public GrpcAuthenticationServiceOptions? GrpcAuthenticationService { get; set; }
}