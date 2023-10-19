using System;
using ReactiveUI;
using Serilog;

namespace Spravy.Ui.Models;

public class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    public RoutableViewModelBase(string? urlPathSegment)
    {
        Log.Logger.Information("Test {Number}", 19);
        UrlPathSegment = urlPathSegment;
        Log.Logger.Information("Test {Number}", 20);
    }

    public string? UrlPathSegment { get; }
    public IScreen HostScreen => throw new NullReferenceException();
}