namespace Spravy.Ui.Services;

public class ModuleViewLocator : IViewLocator
{
    private readonly IKernel resolver;
    
    public ModuleViewLocator(IKernel resolver)
    {
        this.resolver = resolver;
    }
    
    public IViewFor ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel is null)
        {
            return resolver.Get<IViewFor>();
        }

        var type = viewModel.GetType();

        var ns = type.Namespace
           .ThrowIfNullOrWhiteSpace()
           .Replace(".ViewModels.", ".Views.")
           .Replace(".ViewModels", ".Views");

        var viewName = $"{ns}.{type.Name.Substring(0, type.Name.Length - 5)}";
        var viewType = type.Assembly.GetType(viewName).ThrowIfNull(viewName);
        var result = (IViewFor)resolver.Get(viewType);
        result.ViewModel = viewModel;

        return result;
    }
}