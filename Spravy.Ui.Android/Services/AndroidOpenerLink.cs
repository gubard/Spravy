namespace Spravy.Ui.Android.Services;

public class AndroidOpenerLink : IOpenerLink
{
    public Cvtar OpenLinkAsync(Uri link, CancellationToken ct)
    {
        var intent = new Intent(Intent.ActionView, AndroidUri.Parse(link.AbsoluteUri));
        MainActivity.Instance.StartActivity(intent);

        return SpravyResult.AwaitableSuccess;
    }
}