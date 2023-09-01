using System.Linq.Expressions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class DependencyInjectorRegister : IBuilder<DependencyInjector>, IDependencyInjectorRegister
{
    private readonly Dictionary<AutoInjectMemberIdentifier, InjectorItem> autoInjectMembers = new();
    private readonly Dictionary<TypeInformation, InjectorItem> injectors = new();
    private readonly Dictionary<TypeInformation, LazyDependencyInjectorOptions> lazyOptions = new();
    private readonly Dictionary<ReservedCtorParameterIdentifier, InjectorItem> reservedCtorParameters = new();

    public DependencyInjector Build()
    {
        var dependencyInjector = new DependencyInjector(
            injectors,
            autoInjectMembers,
            reservedCtorParameters,
            lazyOptions
        );

        return dependencyInjector;
    }

    public void RegisterConfiguration(IDependencyInjectorConfiguration configuration)
    {
        configuration.Configure(this);
    }

    public void RegisterTransient(Type type, Expression expression)
    {
        RegisterTransientCore(type, expression);
    }

    public void RegisterSingleton(Type type, Expression expression)
    {
        RegisterSingletonCore(type, expression);
    }

    public void RegisterTransientAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    )
    {
        var injectorItem = new InjectorItem(InjectorItemType.Transient, expression);
        autoInjectMembers[memberIdentifier] = injectorItem;
    }

    public void RegisterSingletonAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    )
    {
        var injectorItem = new InjectorItem(InjectorItemType.Singleton, expression);
        autoInjectMembers[memberIdentifier] = injectorItem;
    }

    public void RegisterSingletonReservedCtorParameter(
        ReservedCtorParameterIdentifier identifier,
        Expression expression
    )
    {
        var injectorItem = new InjectorItem(InjectorItemType.Singleton, expression);
        reservedCtorParameters[identifier] = injectorItem;
    }

    public void RegisterTransientReservedCtorParameter(
        ReservedCtorParameterIdentifier identifier,
        Expression expression
    )
    {
        var injectorItem = new InjectorItem(InjectorItemType.Transient, expression);
        reservedCtorParameters[identifier] = injectorItem;
    }

    public void RegisterScope(Type type, Expression expression)
    {
        injectors[type] = new(InjectorItemType.Scope, expression);
    }

    public void SetLazyOptions(TypeInformation type, LazyDependencyInjectorOptions options)
    {
        lazyOptions[type] = options;
    }

    public void RegisterScopeAutoInjectMember(AutoInjectMemberIdentifier memberIdentifier, Expression expression)
    {
        var injectorItem = new InjectorItem(InjectorItemType.Scope, expression);
        autoInjectMembers[memberIdentifier] = injectorItem;
    }

    private void RegisterTransientCore(Type type, Expression expression)
    {
        injectors[type] = new(InjectorItemType.Transient, expression);
    }

    public void RegisterSingletonCore(Type type, Expression expression)
    {
        injectors[type] = new(InjectorItemType.Singleton, expression);
    }
}