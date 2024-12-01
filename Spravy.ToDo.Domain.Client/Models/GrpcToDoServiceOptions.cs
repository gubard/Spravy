namespace Spravy.ToDo.Domain.Client.Models;

public class GrpcToDoServiceOptions : IGrpcOptionsValue
{
    public static string Section => "GrpcToDoService";

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}

public class GrpcToDoServiceOptionsConfiguration
{
    public GrpcToDoServiceOptions? GrpcToDoService { get; set; }
}