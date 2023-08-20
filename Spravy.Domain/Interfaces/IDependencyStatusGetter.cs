using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IDependencyStatusGetter
{
    DependencyStatus GetStatus(
        TypeInformation type,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    );
}