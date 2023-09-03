using Ninject;
using ReactiveUI;
using Spravy.Domain.Extensions;

namespace Spravy.Ui.Services;

public class ModuleViewLocator : IViewLocator
{
    [Inject]
    public required IKernel Resolver { get; init; }

    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel is null)
        {
            return Resolver.Get<IViewFor>();
        }

        var type = viewModel.GetType();

        var ns = type.Namespace
            .Replace(".ViewModels.", ".Views.")
            .Replace(".ViewModels", ".Views");

        var viewName = $"{ns}.{type.Name.Substring(0, type.Name.Length - 5)}";
        var viewType = type.Assembly.GetType(viewName).ThrowIfNull(viewName);
        var result = (IViewFor)Resolver.Get(viewType);

        if (viewModel is not null)
        {
            result.ViewModel = viewModel;
        }

        return result;
    }
}