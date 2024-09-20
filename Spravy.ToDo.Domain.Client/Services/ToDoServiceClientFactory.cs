namespace Spravy.ToDo.Domain.Client.Services;

public class ToDoServiceClientFactory : IFactory<ChannelBase, ToDoService.ToDoServiceClient>
{
    public Result<ToDoService.ToDoServiceClient> Create(ChannelBase key)
    {
        return new(new ToDoService.ToDoServiceClient(key));
    }
}
