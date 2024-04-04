using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface INavigatable : ISaveState
{
    bool IsPooled { get; }
    string ViewId { get; }

    Result Stop();
    ValueTask<Result> SetStateAsync(object setting);
}