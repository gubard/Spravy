using Spravy.Client.Enums;
using Spravy.Client.Interfaces;

namespace Spravy.PasswordGenerator.Domain.Client.Models;

public class GrpcPasswordServiceOptions : IGrpcOptionsValue
{
    public static string Section => "GrpcPasswordService";
    
    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }    
}