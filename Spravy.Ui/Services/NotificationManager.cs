using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Ninject;
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

    public async Task ShowAsync<TView>(CancellationToken cancellationToken)
    {
        var view = kernel.Get<TView>();
        await this.InvokeUIBackgroundAsync(() => managedNotificationManager.Show(view));
    }
}