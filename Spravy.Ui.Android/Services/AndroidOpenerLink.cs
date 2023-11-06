using System;
using System.Threading.Tasks;
using Android.Content;
using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Android.Services;

public class AndroidOpenerLink : IOpenerLink
{
    private readonly ContextWrapper contextWrapper;

    public AndroidOpenerLink(ContextWrapper contextWrapper)
    {
        this.contextWrapper = contextWrapper;
    }

    public Task OpenLinkAsync(Uri link)
    {
        var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(link.AbsoluteUri));
        contextWrapper.StartActivity(intent);

        return Task.CompletedTask;
    }
}