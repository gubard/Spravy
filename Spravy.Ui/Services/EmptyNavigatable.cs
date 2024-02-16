using System.Threading.Tasks;
using Spravy.Domain.Helpers;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class EmptyNavigatable : INavigatable
{
    public bool IsPooled => false;
    public string ViewId => TypeCache<EmptyNavigatable>.Type.Name;

    public Task SaveStateAsync()
    {
        return Task.CompletedTask;
    }

    public void Stop()
    {
    }

    public Task SetStateAsync(object setting)
    {
        return Task.CompletedTask;
    }
}