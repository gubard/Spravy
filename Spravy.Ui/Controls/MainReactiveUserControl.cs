using Avalonia.ReactiveUI;
using Spravy.Domain.Extensions;

namespace Spravy.Ui.Controls;

public class MainReactiveUserControl<T> : ReactiveUserControl<T> where T : class
{
    public T MainViewModel => ViewModel.ThrowIfNull();
}