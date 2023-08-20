using ReactiveUI;
using Splat;
using Spravy.Domain.Interfaces;

namespace Spravy.Ui.Services;

public class ReactiveUIModuleSetup : IModuleSetup
{
    public void Setup(IViewLocator viewLocator)
    {
        Locator.CurrentMutable.RegisterLazySingleton(() => viewLocator, typeof(IViewLocator));
    }
}