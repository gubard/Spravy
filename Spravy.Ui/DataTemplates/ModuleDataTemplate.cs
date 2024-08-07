namespace Spravy.Ui.DataTemplates;

public class ModuleDataTemplate : IDataTemplate
{
    private readonly IViewSelector viewSelector;
    private readonly IServiceFactory serviceFactory;

    public ModuleDataTemplate(IViewSelector viewSelector, IServiceFactory serviceFactory)
    {
        this.viewSelector = viewSelector;
        this.serviceFactory = serviceFactory;
    }

    public Control Build(object? param)
    {
        var type = param.ThrowIfNull().GetType();
        var view = viewSelector.GetView(type);

        if (!view.TryGetValue(out var value))
        {
            return CreateErrorView(view.Errors);
        }

        value.DataContext = param;
        var cast = value.CastObject<Control>();

        if (!cast.TryGetValue(out var control))
        {
            return CreateErrorView(cast.Errors);
        }

        return control;
    }

    private ErrorView CreateErrorView(ReadOnlyMemory<Error> errors)
    {
        var errorView = serviceFactory.CreateService<ErrorView>();
        var viewModel = serviceFactory.CreateService<ErrorViewModel>();
        viewModel.Errors.AddRange(errors.ToArray());
        errorView.DataContext = viewModel;

        return errorView;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
