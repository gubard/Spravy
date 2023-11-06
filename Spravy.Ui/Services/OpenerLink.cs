using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Services;

public class OpenerLink : IOpenerLink
{
    public Task OpenLinkAsync(Uri link)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var url = link.AbsoluteUri.Replace("&", "^&");

            Process.Start(
                new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                }
            );
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", link.AbsoluteUri);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", link.AbsoluteUri);
        }

        throw new NotSupportedException();
    }
}