using System;
using ReactiveUI;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ExceptionViewModel : NavigatableViewModelBase
{
    private Exception? exception;
    private string? fullExceptionText;

    public ExceptionViewModel() : base(true)
    {
        this.WhenAnyValue(x => x.Exception).Subscribe(x => FullExceptionText = x?.ToString());
    }

    public Exception? Exception
    {
        get => exception;
        set => this.RaiseAndSetIfChanged(ref exception, value);
    }

    public string? FullExceptionText
    {
        get => fullExceptionText;
        set => this.RaiseAndSetIfChanged(ref fullExceptionText, value);
    }
}