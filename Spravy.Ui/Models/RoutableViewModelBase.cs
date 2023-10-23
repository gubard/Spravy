using System;
using ReactiveUI;

namespace Spravy.Ui.Models;

public class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
{
    public RoutableViewModelBase(string? urlPathSegment)
    {
        UrlPathSegment = urlPathSegment;
    }

    public string? UrlPathSegment { get; }
    public IScreen HostScreen => throw new NullReferenceException();
}