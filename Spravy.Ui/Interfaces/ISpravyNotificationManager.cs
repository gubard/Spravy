using System.Threading;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    Task ShowAsync<TView>(CancellationToken cancellationToken);
}