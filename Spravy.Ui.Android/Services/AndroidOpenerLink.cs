using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Android.Content;
using Spravy.Domain.Models;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Android.Services;

public class AndroidOpenerLink : IOpenerLink
{
    private readonly ContextWrapper contextWrapper;

    public AndroidOpenerLink(ContextWrapper contextWrapper)
    {
        this.contextWrapper = contextWrapper;
    }

    public ConfiguredValueTaskAwaitable<Result> OpenLinkAsync(Uri link, CancellationToken cancellationToken)
    {
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(link.AbsoluteUri));
        contextWrapper.StartActivity(intent);

        return Result.AwaitableFalse;
    }
}