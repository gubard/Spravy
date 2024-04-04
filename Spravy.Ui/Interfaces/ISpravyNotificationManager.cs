using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    ValueTask<Result> ShowAsync<TView>(CancellationToken cancellationToken);
    ValueTask<Result> ShowAsync(object view,CancellationToken cancellationToken);
}