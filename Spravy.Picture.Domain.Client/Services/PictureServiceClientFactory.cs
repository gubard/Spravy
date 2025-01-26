using Grpc.Core;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Picture.Protos;

namespace Spravy.Picture.Domain.Client.Services;

public class PictureServiceClientFactory : IFactory<ChannelBase, PictureService.PictureServiceClient>
{
    public Result<PictureService.PictureServiceClient> Create(ChannelBase key)
    {
        return new(new PictureService.PictureServiceClient(key));
    }
}