using System.Runtime.CompilerServices;
using System.Threading;
using Spravy.Domain.Models;

namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    ConfiguredValueTaskAwaitable<Result> ShowAsync<TView>(CancellationToken cancellationToken);
    ConfiguredValueTaskAwaitable<Result> ShowAsync(object view,CancellationToken cancellationToken);
}