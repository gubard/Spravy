namespace Spravy.Ui.Android.Services;

public class AndroidOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable<Spravy.Domain.Models.Result> OpenLinkAsync(
        Uri link,
        CancellationToken ct
    )
    {
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(link.AbsoluteUri));
        MainActivity.Instance.StartActivity(intent);

        return Domain.Models.Result.AwaitableSuccess;
    }
}
