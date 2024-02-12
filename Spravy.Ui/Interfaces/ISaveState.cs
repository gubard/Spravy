using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface ISaveState
{
    Task SaveStateAsync();
}