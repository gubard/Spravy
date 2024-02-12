using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface INavigatable : ISaveState
{
    bool IsPooled { get; }
    string ViewId { get; }

    void Stop();
    Task SetStateAsync(object setting);
}