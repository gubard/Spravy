using Spravy.Client.Enums;
using Spravy.Client.Interfaces;

namespace Spravy.Picture.Domain.Client.Models;

public class GrpcPictureServiceOptions : IGrpcOptionsValue
{
    public static string Section => "GrpcPictureService";

    public string? Host { get; set; }
    public GrpcChannelType ChannelType { get; set; }
    public ChannelCredentialType ChannelCredentialType { get; set; }
    public string? Token { get; set; }
}