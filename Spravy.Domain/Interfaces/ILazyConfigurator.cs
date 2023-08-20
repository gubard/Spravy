using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface ILazyConfigurator
{
    void SetLazyOptions(TypeInformation type, LazyDependencyInjectorOptions options);
}