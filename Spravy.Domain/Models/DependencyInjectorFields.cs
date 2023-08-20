using System.Diagnostics;
using System.Linq.Expressions;
using Spravy.Domain.Extensions;

namespace Spravy.Domain.Models;

public readonly struct DependencyInjectorFields
{
    public readonly Dictionary<AutoInjectMemberIdentifier, InjectorItem> AutoInjectMembers;
    public readonly HashSet<Type> CheckedInjectAttribute;
    public readonly Dictionary<TypeInformation, InjectorItem> Injectors;
    public readonly Dictionary<TypeInformation, Expression> CacheSingleton;
    public readonly Dictionary<TypeInformation, LazyDependencyInjectorOptions> LazyOptions;
    public readonly ReadOnlyMemory<TypeInformation> Inputs;
    public readonly ReadOnlyMemory<TypeInformation> Outputs;

    public readonly Dictionary<
        ReservedCtorParameterIdentifier,
        InjectorItem
    > ReservedCtorParameters;

    public DependencyInjectorFields(
        IReadOnlyDictionary<TypeInformation, InjectorItem> injectors,
        IReadOnlyDictionary<AutoInjectMemberIdentifier, InjectorItem> autoInjects,
        IReadOnlyDictionary<ReservedCtorParameterIdentifier, InjectorItem> reservedCtorParameters,
        IReadOnlyDictionary<TypeInformation, LazyDependencyInjectorOptions> lazyOptions
    )
    {
        CheckedInjectAttribute = new ();
        CacheSingleton = new ();
        ReservedCtorParameters = new (reservedCtorParameters);
        Injectors = new (injectors);
        AutoInjectMembers = new (autoInjects);
        LazyOptions = new (lazyOptions);
        var array = Injectors.Select(x => x.Key).OrderBy(x => x.ToString()).ToArray();

        Outputs = array;

        Inputs = GetInputs(Injectors, AutoInjectMembers)
            .Distinct()
            .Where(x => !array.Contains(x) && !x.Type.IsClosure())
            .ToArray();
    }

    private IEnumerable<TypeInformation> GetInputs(
        IReadOnlyDictionary<TypeInformation, InjectorItem> injectors,
        IReadOnlyDictionary<AutoInjectMemberIdentifier, InjectorItem> autoInjects
    )
    {
        foreach (var value in GetInputs(injectors))
        {
            yield return value;
        }

        foreach (var value in GetInputs(autoInjects))
        {
            yield return value;
        }
    }

    private IEnumerable<TypeInformation> GetInputs(
        IReadOnlyDictionary<AutoInjectMemberIdentifier, InjectorItem> autoInjects
    )
    {
        foreach (var autoInject in autoInjects)
        foreach (var value in GetInputs(autoInject.Value.Expression))
        {
            yield return value;
        }
    }

    private IEnumerable<TypeInformation> GetInputs(
        IReadOnlyDictionary<TypeInformation, InjectorItem> injectors
    )
    {
        foreach (var injector in injectors)
        foreach (var value in GetInputs(injector.Value.Expression))
        {
            yield return value;
        }
    }

    private IEnumerable<TypeInformation> GetInputs(Expression expression)
    {
        switch (expression)
        {
            case NewExpression newExpression:
            {
                foreach (var argument in newExpression.Arguments)
                {
                    if (argument is ParameterExpression)
                    {
                        yield return argument.Type;

                        continue;
                    }

                    foreach (var input in GetInputs(argument))
                    {
                        yield return input;
                    }
                }

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var parameter in lambdaExpression.Parameters)
                {
                    yield return parameter.Type;
                }

                foreach (var input in GetInputs(lambdaExpression.Body))
                {
                    yield return input;
                }

                break;
            }
            case NewArrayExpression newArrayExpression:
            {
                foreach (var value in newArrayExpression.Expressions)
                foreach (var input in GetInputs(value))
                {
                    yield return input;
                }

                break;
            }
            case ParameterExpression:
            {
                yield return expression.Type;

                break;
            }
            case ConstantExpression:
            {
                break;
            }
            case MemberExpression:
            {
                break;
            }
            case MemberInitExpression memberInitExpression:
            {
                foreach (var input in GetInputs(memberInitExpression.NewExpression))
                {
                    yield return input;
                }

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var argument in methodCallExpression.Arguments)
                foreach (var input in GetInputs(argument))
                {
                    yield return input;
                }

                if (methodCallExpression.Object is null)
                {
                    break;
                }

                foreach (var input in GetInputs(methodCallExpression.Object))
                {
                    yield return input;
                }

                break;
            }
            case UnaryExpression unaryExpression:
            {
                foreach (var input in GetInputs(unaryExpression.Operand))
                {
                    yield return input;
                }

                break;
            }
            case ListInitExpression listInitExpression:
            {
                foreach (var input in GetInputs(listInitExpression.NewExpression))
                {
                    yield return input;
                }

                foreach (var initializer in listInitExpression.Initializers)
                foreach (var argument in initializer.Arguments)
                foreach (var input in GetInputs(argument))
                {
                    yield return input;
                }

                break;
            }
            default:
            {
                var type = expression.GetType();

                throw new UnreachableException(type.ToString());
            }
        }
    }
}