using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Android.Content;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Android.Services;

public class AndroidOpenerLink : IOpenerLink
{
    public ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(link.AbsoluteUri));
        MainActivity.Instance.StartActivity(intent);

        return Result.AwaitableSuccess;
    }
}