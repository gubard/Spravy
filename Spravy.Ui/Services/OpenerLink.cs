namespace Spravy.Ui.Services;

public class OpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var url = link.AbsoluteUri.Replace("&", "^&");

            var info = new ProcessStartInfo(url)
            {
                UseShellExecute = true,
            };

            Process.Start(info);

            return Result.AwaitableFalse;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", link.AbsoluteUri);

            return Result.AwaitableFalse;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", link.AbsoluteUri);

            return Result.AwaitableFalse;
        }

        throw new NotSupportedException();
    }
}