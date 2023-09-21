using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;
using Spravy.Ui.Models;

namespace Spravy.Ui.Services;

public class ModuleDataTemplate : IDataTemplate
{
    [Inject]
    public required IKernel Resolver { get; init; }

    public Control? Build(object? param)
    {
        var type = param.GetType();

        var ns = type.Namespace.ThrowIfNullOrWhiteSpace()
            .Replace(".ViewModels.", ".Views.")
            .Replace(".ViewModels", ".Views");

        var viewName = $"{ns}.{type.Name.Substring(0, type.Name.Length - 5)}";
        var viewType = type.Assembly.GetType(viewName).ThrowIfNull(viewName);
        var view = (Control)Resolver.Get(viewType);

        if (view is IViewFor viewFor)
        {
            viewFor.ViewModel = param;
        }

        return view;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}