namespace Spravy.Ui.DataTemplates;

public class ModuleDataTemplate : IDataTemplate
{
    private readonly IServiceFactory serviceFactory;
    
    public ModuleDataTemplate(IServiceFactory serviceFactory)
    {
        this.serviceFactory = serviceFactory;
    }
    
    public Control Build(object? param)
    {
        var type = param.ThrowIfNull().GetType();

        var ns = type.Namespace
           .ThrowIfNullOrWhiteSpace()
           .Replace(".ViewModels.", ".Views.")
           .Replace(".ViewModels", ".Views");

        var viewName = $"{ns}.{type.Name.Substring(0, type.Name.Length - 5)}";
        var viewType = type.Assembly.GetType(viewName).ThrowIfNull(viewName);
        var view = (Control)serviceFactory.CreateService(viewType);

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