using System.Linq.Expressions;
using System.Reflection;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class MutDependencyInjector : IMutDependencyInjector
{
    private readonly DependencyInjectorRegister register;

    public MutDependencyInjector()
    {
        register = new ();
    }

    public ReadOnlyMemory<TypeInformation> Inputs => CreateDependencyInjector().Inputs;
    public ReadOnlyMemory<TypeInformation> Outputs => CreateDependencyInjector().Outputs;

    public object Resolve(TypeInformation type)
    {
        return CreateDependencyInjector().Resolve(type);
    }

    public object? Invoke(Delegate del, DictionarySpan<TypeInformation, object> arguments)
    {
        return CreateDependencyInjector().Invoke(del, arguments);
    }

    public object? Invoke(object? obj, MethodInfo method, DictionarySpan<TypeInformation, object> arguments)
    {
        return CreateDependencyInjector().Invoke(obj, method, arguments);
    }

    public DependencyStatus GetStatus(
        TypeInformation type,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        return CreateDependencyInjector().GetStatus(type, scopeParameters);
    }

    public void RegisterTransient(Type type, Expression expression)
    {
        register.RegisterTransient(type, expression);
    }

    public void RegisterSingleton(Type type, Expression expression)
    {
        register.RegisterSingleton(type, expression);
    }

    public void RegisterTransientAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    )
    {
        register.RegisterTransientAutoInjectMember(memberIdentifier, expression);
    }

    public void RegisterSingletonAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    )
    {
        register.RegisterSingletonAutoInjectMember(memberIdentifier, expression);
    }

    public void RegisterConfiguration(IDependencyInjectorConfiguration configuration)
    {
        register.RegisterConfiguration(configuration);
    }

    public void RegisterSingletonReservedCtorParameter(
        ReservedCtorParameterIdentifier identifier,
        Expression expression
    )
    {
        register.RegisterSingletonReservedCtorParameter(identifier, expression);
    }

    public void RegisterTransientReservedCtorParameter(
        ReservedCtorParameterIdentifier identifier,
        Expression expression
    )
    {
        register.RegisterTransientReservedCtorParameter(identifier, expression);
    }

    public void RegisterScope(Type type, Expression expression)
    {
        register.RegisterScope(type, expression);
    }

    public void SetLazyOptions(TypeInformation type, LazyDependencyInjectorOptions options)
    {
        register.SetLazyOptions(type, options);
    }

    public void RegisterScopeAutoInjectMember(AutoInjectMemberIdentifier memberIdentifier, Expression expression)
    {
        register.RegisterScopeAutoInjectMember(memberIdentifier, expression);
    }

    private IDependencyInjector CreateDependencyInjector()
    {
        return register.Build();
    }
}