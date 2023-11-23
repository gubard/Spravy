using System.Threading;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface IRefresh
{
    Task RefreshAsync(CancellationToken cancellationToken);
}