using Grpc.Core;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.ToDo.Protos;

namespace Spravy.ToDo.Domain.Client.Services;

public class ToDoServiceClientFactory : IFactory<ChannelBase, ToDoService.ToDoServiceClient>
{
    public Result<ToDoService.ToDoServiceClient> Create(ChannelBase key)
    {
        return new(new ToDoService.ToDoServiceClient(key));
    }
}
