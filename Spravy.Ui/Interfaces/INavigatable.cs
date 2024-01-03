using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface INavigatable
{
    bool IsPooled { get; }
    string ViewId { get; }
    
    void Stop();
    Task SaveStateAsync();
    Task SetStateAsync(object setting);
}