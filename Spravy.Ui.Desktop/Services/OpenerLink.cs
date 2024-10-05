using System.Diagnostics;
using System.Runtime.InteropServices;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Desktop.Services;

public class OpenerLink : IOpenerLink
{
    public Cvtar OpenLinkAsync(Uri link, CancellationToken ct)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var url = link.AbsoluteUri.Replace("&", "^&");

            var info = new ProcessStartInfo(url) { UseShellExecute = true };

            Process.Start(info);

            return Result.AwaitableSuccess;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", link.AbsoluteUri);

            return Result.AwaitableSuccess;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", link.AbsoluteUri);

            return Result.AwaitableSuccess;
        }

        throw new NotSupportedException();
    }
}
