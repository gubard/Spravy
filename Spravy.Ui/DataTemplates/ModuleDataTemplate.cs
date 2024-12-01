namespace Spravy.Ui.DataTemplates;

public class ModuleDataTemplate : IDataTemplate
{
    private readonly IServiceFactory serviceFactory;
    private readonly IViewFactory viewFactory;
    private readonly IViewSelector viewSelector;

    public ModuleDataTemplate(IViewSelector viewSelector, IServiceFactory serviceFactory, IViewFactory viewFactory)
    {
        this.viewSelector = viewSelector;
        this.serviceFactory = serviceFactory;
        this.viewFactory = viewFactory;
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
        var cast = value.CastObject<Control>(nameof(value));

        if (!cast.TryGetValue(out var control))
        {
            return CreateErrorView(cast.Errors);
        }

        return control;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }

    private ErrorView CreateErrorView(ReadOnlyMemory<Error> errors)
    {
        var errorView = serviceFactory.CreateService<ErrorView>();
        var viewModel = viewFactory.CreateErrorViewModel(errors);
        errorView.DataContext = viewModel;

        return errorView;
    }
}