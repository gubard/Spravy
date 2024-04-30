using Avalonia.ReactiveUI;
using Spravy.Domain.Extensions;

namespace Spravy.Ui.Controls;

public abstract class MainReactiveUserControl<T> : ReactiveUserControl<T> where T : class
{
    public T MainViewModel
    {
        get => ViewModel.ThrowIfNull();
    }
}