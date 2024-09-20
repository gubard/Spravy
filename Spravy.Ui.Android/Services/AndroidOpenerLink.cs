using System;
using System.Threading;

namespace Spravy.Ui.Android.Services;

public class AndroidOpenerLink : IOpenerLink
{
    public Cvtar OpenLinkAsync(Uri link, CancellationToken ct)
    {
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(link.AbsoluteUri));
        MainActivity.Instance.StartActivity(intent);

        return Result.AwaitableSuccess;
    }
}
