using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ExceptionViewModel : ViewModelBase
{
    public ExceptionViewModel()
    {
        this.WhenAnyValue(x => x.Exception).Subscribe(x => FullExceptionText = x?.ToString());
    }

    [Reactive]
    public Exception? Exception { get; set; }

    [Reactive]
    public string? FullExceptionText { get; set; }
}