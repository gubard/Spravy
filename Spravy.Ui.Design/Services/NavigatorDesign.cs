using Spravy.Ui.Interfaces;

namespace Spravy.Ui.Design.Services;

public class NavigatorDesign : INavigator
{
    public Task<INavigatable?> NavigateBackAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync<TViewModel>(TViewModel parameter, CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync(Type type, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task NavigateToAsync<TViewModel>(Action<TViewModel> setup, CancellationToken cancellationToken)
        where TViewModel : INavigatable
    {
        throw new NotImplementedException();
    }
}