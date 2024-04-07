using System.Runtime.CompilerServices;
using System.Threading;
using Avalonia.Controls.Notifications;
using Ninject;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class NotificationManager : ISpravyNotificationManager
{
    private readonly IManagedNotificationManager managedNotificationManager;
    private readonly IKernel kernel;

    public NotificationManager(IManagedNotificationManager managedNotificationManager, IKernel kernel)
    {
        this.managedNotificationManager = managedNotificationManager;
        this.kernel = kernel;
    }

    public ConfiguredValueTaskAwaitable<Result> ShowAsync<TView>(CancellationToken cancellationToken)
    {
        var view = kernel.Get<TView>();

        return this.InvokeUIBackgroundAsync(() => managedNotificationManager.Show(view));
    }

    public ConfiguredValueTaskAwaitable<Result> ShowAsync(object view, CancellationToken cancellationToken)
    {
        return this.InvokeUIBackgroundAsync(() => managedNotificationManager.Show(view));
    }
}