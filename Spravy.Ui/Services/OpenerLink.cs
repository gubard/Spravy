using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class OpenerLink : IOpenerLink
{
    public ValueTask<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var url = link.AbsoluteUri.Replace("&", "^&");

            var info = new ProcessStartInfo(url)
            {
                UseShellExecute = true
            };

            cancellationToken.ThrowIfCancellationRequested();
            Process.Start(info);

            return Result.SuccessValueTask;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", link.AbsoluteUri);

            return Result.SuccessValueTask;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", link.AbsoluteUri);

            return Result.SuccessValueTask;
        }

        throw new NotSupportedException();
    }
}