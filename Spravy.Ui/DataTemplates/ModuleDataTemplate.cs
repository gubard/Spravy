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

        if (view.IsHasError)
        {
            var errorView = serviceFactory.CreateService<ErrorView>();
            errorView.ViewModel = serviceFactory.CreateService<ErrorViewModel>();
            errorView.ViewModel.Errors.AddRange(view.Errors.ToArray());

            return errorView;
        }

        view.Value.ViewModel = param;

        return view.Value.ThrowIfIsNotCast<Control>();
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
