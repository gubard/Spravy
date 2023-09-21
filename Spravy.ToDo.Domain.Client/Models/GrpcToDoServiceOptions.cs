using Spravy.Client.Enums;
using Spravy.Domain.Interfaces;

namespace Spravy.ToDo.Domain.Client.Models;

public class GrpcToDoServiceOptions : IOptionsValue
{
    public static string Section => "GrpcToDoService";
    
    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}