using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Spravy.Domain.Attributes;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using AutoInjectMemberIdentifier = Spravy.Domain.Models.AutoInjectMemberIdentifier;
using DependencyInjectorFields = Spravy.Domain.Models.DependencyInjectorFields;
using DependencyStatus = Spravy.Domain.Models.DependencyStatus;
using LazyDependencyInjectorOptions = Spravy.Domain.Models.LazyDependencyInjectorOptions;
using ReservedCtorParameterIdentifier = Spravy.Domain.Models.ReservedCtorParameterIdentifier;
using ScopeValue = Spravy.Domain.Models.ScopeValue;
using TypeInformation = Spravy.Domain.Models.TypeInformation;

namespace Spravy.Domain.Services;

public class DependencyInjector : IDependencyInjector
{
    private readonly DependencyInjectorFields fields;
    public static IResolver? Default;
    
    
    public DependencyInjector(
        IReadOnlyDictionary<TypeInformation, Models.InjectorItem> injectors,
        IReadOnlyDictionary<AutoInjectMemberIdentifier, Models.InjectorItem> autoInjects,
        IReadOnlyDictionary<ReservedCtorParameterIdentifier, Models.InjectorItem> reservedCtorParameters,
        IReadOnlyDictionary<TypeInformation, LazyDependencyInjectorOptions> lazyOptions
    )
    {
        Check(injectors);

        fields = new (
            injectors,
            autoInjects,
            reservedCtorParameters,
            lazyOptions
        );
    }

    public ReadOnlyMemory<TypeInformation> Inputs => fields.Inputs;
    public ReadOnlyMemory<TypeInformation> Outputs => fields.Outputs;

    public object Resolve(TypeInformation type)
    {
        if (!fields.Injectors.TryGetValue(type, out var injectorItem))
        {
            throw new TypeNotRegisterException(type.Type);
        }

        BuildExpression(
            type,
            injectorItem,
            new (),
            out var expression
        );

        var func = BuildFunc(expression);
        var value = func.Invoke();

        return value;
    }

    public DependencyStatus GetStatus(
        TypeInformation type,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        if (IsLazy(type))
        {
            return GetLazyStatus(type, scopeParameters);
        }

        if (!fields.Injectors.TryGetValue(type, out var injectorItem))
        {
            throw new TypeNotRegisterException(type.Type);
        }

        BuildExpression(type, injectorItem, scopeParameters, out var expression);

        return new (type, expression);
    }

    public object? Invoke(Delegate del, Models.DictionarySpan<TypeInformation, object> arguments)
    {
        var parameterTypes = del.GetParameterTypes();
        var args = new object[parameterTypes.Length];

        for (var index = 0; index < args.Length; index++)
        {
            if (arguments.TryGetValue(parameterTypes[index], out var value))
            {
                args[index] = value;
            }
            else
            {
                args[index] = Resolve(parameterTypes[index]);
            }
        }

        return del.DynamicInvoke(args);
    }

    public object? Invoke(object? obj, MethodInfo method, Models.DictionarySpan<TypeInformation, object> arguments)
    {
        var parameterTypes = method.GetParameters();
        var args = new object[parameterTypes.Length];

        for (var index = 0; index < args.Length; index++)
        {
            if (arguments.TryGetValue(parameterTypes[index].ParameterType, out var value))
            {
                args[index] = value;
            }
            else
            {
                args[index] = Resolve(parameterTypes[index].ParameterType);
            }
        }

        return method.Invoke(obj, args);
    }

    private bool BuildExpression(
        TypeInformation type,
        Models.InjectorItem injectorItem,
        Dictionary<TypeInformation, ScopeValue> scopeExpressions,
        out Expression result
    )
    {
        switch (injectorItem.Type)
        {
            case InjectorItemType.Singleton:
            {
                if (fields.CacheSingleton.TryGetValue(type, out result))
                {
                    return true;
                }

                if (UpdateParameters(type, injectorItem.Expression, scopeExpressions, out result))
                {
                    var constant = result
                        .ToLambda()
                        .Compile()
                        .DynamicInvoke()
                        .ThrowIfNull()
                        .ToConstant();

                    result = constant;
                    fields.CacheSingleton.Add(type, constant);

                    return true;
                }

                return false;
            }
            case InjectorItemType.Transient:
            {
                var isFull = UpdateParameters(
                    type,
                    injectorItem.Expression,
                    scopeExpressions,
                    out result
                );

                return isFull;
            }
            case InjectorItemType.Scope:
            {
                if (scopeExpressions.TryGetValue(type, out var value))
                {
                    result = value.Parameter;

                    return true;
                }

                var isFull = UpdateParameters(
                    type,
                    injectorItem.Expression,
                    scopeExpressions,
                    out result
                );

                var variable = result.Type.ToVariableAutoName();
                scopeExpressions.TryAdd(type, new (variable, result));
                scopeExpressions.TryAdd(variable.Type, new (variable, result));
                result = variable;

                return isFull;
            }
            default:
            {
                throw new UnreachableException();
            }
        }
    }

    private Expression ValueTypeValidate(TypeInformation type, Expression expression)
    {
        if (type.IsValueType && type != expression.Type)
        {
            return expression.ToConvert(type.Type);
        }

        return expression;
    }

    private bool UpdateParameters(
        TypeInformation type,
        Expression expression,
        Dictionary<TypeInformation, ScopeValue> scopeParameters,
        out Expression result
    )
    {
        var isFull = true;

        switch (expression)
        {
            case InvocationExpression invocationExpression:
            {
                var arguments = new List<Expression>();

                foreach (var argument in invocationExpression.Arguments)
                {
                    if (!UpdateParameters(argument.Type, argument, scopeParameters, out var value))
                    {
                        isFull = false;
                    }

                    arguments.Add(value);
                }

                result = invocationExpression.Update(invocationExpression.Expression, arguments);

                break;
            }
            case ParameterExpression parameterExpression:
            {
                if (scopeParameters.TryGetValue(parameterExpression.Type, out var value))
                {
                    result = value.Parameter;

                    break;
                }

                var parameter = CreateParameter(parameterExpression, scopeParameters);

                if (parameter is ParameterExpression)
                {
                    isFull = false;
                }

                result = parameter;

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                var expressions = new List<Expression>();

                foreach (var parameter in lambdaExpression.Parameters)
                {
                    if (
                        !UpdateParameters(parameter.Type, parameter, scopeParameters, out var value)
                    )
                    {
                        isFull = false;
                    }

                    expressions.Add(value);
                }

                result = lambdaExpression.ToInvoke(expressions);

                break;
            }
            case NewExpression newExpression:
            {
                var arguments = new List<Expression>();

                var parameters = newExpression.Constructor is null
                    ? Array.Empty<ParameterInfo>()
                    : newExpression.Constructor.GetParameters();

                var reserveds =
                    new Dictionary<int, (TypeInformation Type, Models.InjectorItem InjectorItem)>();

                for (var index = 0; index < parameters.Length; index++)
                {
                    var identifier = new ReservedCtorParameterIdentifier(
                        newExpression.Type,
                        newExpression.Constructor.ThrowIfNull(),
                        parameters[index]
                    );

                    if (
                        !fields.ReservedCtorParameters.TryGetValue(identifier, out var injectorItem)
                    )
                    {
                        continue;
                    }

                    reserveds.Add(index, (identifier.Parameter.ParameterType, injectorItem));
                }

                for (var index = 0; index < newExpression.Arguments.Count; index++)
                {
                    if (reserveds.TryGetValue(index, out var item))
                    {
                        if (
                            !BuildExpression(
                                item.Type,
                                item.InjectorItem,
                                scopeParameters,
                                out var reserved
                            )
                        )
                        {
                            isFull = false;
                        }

                        arguments.Add(reserved);

                        continue;
                    }

                    var argument = newExpression.Arguments[index];

                    if (!UpdateParameters(argument.Type, argument, scopeParameters, out var value))
                    {
                        isFull = false;
                    }

                    arguments.Add(value);
                }

                result = newExpression.Update(arguments);

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                var arguments = new List<Expression>();

                foreach (var argument in methodCallExpression.Arguments)
                {
                    if (!UpdateParameters(argument.Type, argument, scopeParameters, out var value))
                    {
                        isFull = false;
                    }

                    arguments.Add(value);
                }

                if (methodCallExpression.Object is null)
                {
                    result = methodCallExpression.Update(methodCallExpression.Object, arguments);
                }
                else
                {
                    if (
                        !UpdateParameters(
                            methodCallExpression.Object.Type,
                            methodCallExpression.Object,
                            scopeParameters,
                            out var obj
                        )
                    )
                    {
                        isFull = false;
                    }

                    result = methodCallExpression.Update(obj, arguments);
                }

                break;
            }
            case ConstantExpression constantExpression:
            {
                result = constantExpression;

                break;
            }
            case BlockExpression blockExpression:
            {
                var expressions = new List<Expression>();

                foreach (var blockItem in blockExpression.Expressions)
                {
                    if (!UpdateParameters(blockItem.Type, blockItem, scopeParameters, out var obj))
                    {
                        isFull = false;
                    }

                    expressions.Add(obj);
                }

                result = blockExpression.Update(blockExpression.Variables, expressions);

                break;
            }
            case BinaryExpression binaryExpression:
            {
                LambdaExpression? conversion = null;

                if (binaryExpression.Conversion is not null)
                {
                    if (
                        !UpdateParameters(
                            binaryExpression.Conversion.Type,
                            binaryExpression.Conversion,
                            scopeParameters,
                            out var conversionUpdated
                        )
                    )
                    {
                        isFull = false;
                    }

                    conversion = (LambdaExpression)conversionUpdated;
                }

                if (
                    !UpdateParameters(
                        binaryExpression.Type,
                        binaryExpression.Right,
                        scopeParameters,
                        out var right
                    )
                )
                {
                    isFull = false;
                }

                result = binaryExpression.Update(binaryExpression.Left, conversion, right);

                break;
            }
            default:
            {
                var expressionType = expression.GetType();

                throw new UnreachableException(expressionType.ToString());
            }
        }

        if (!AutoInjectMembers(result, scopeParameters, out result))
        {
            isFull = false;
        }

        result = ValueTypeValidate(type, result);

        return isFull;
    }

    private bool AutoInjectMembers(
        Expression root,
        Dictionary<TypeInformation, ScopeValue> scopeParameters,
        out Expression result
    )
    {
        var isFull = true;
        var variables = new List<ParameterExpression>();
        var members = root.Type.GetMembers();
        var memberExpressions = new List<(MemberInfo Member, Expression Expression)>();

        foreach (var member in members)
        {
            var identifier = new AutoInjectMemberIdentifier(root.Type, member);

            if (!fields.AutoInjectMembers.TryGetValue(identifier, out var injectorItem))
            {
                if (fields.CheckedInjectAttribute.Contains(root.Type))
                {
                    continue;
                }

                var injectAttribute = member.GetCustomAttribute<InjectAttribute>();

                if (injectAttribute is null)
                {
                    continue;
                }

                switch (member)
                {
                    case PropertyInfo property:
                    {
                        var parameter = property.PropertyType.ToParameterAutoName();
                        injectorItem = new (InjectorItemType.Scope, parameter.ToLambda(parameter));

                        fields.AutoInjectMembers.Add(identifier, injectorItem);

                        break;
                    }
                    case FieldInfo field:
                    {
                        var parameter = field.FieldType.ToParameterAutoName();
                        injectorItem = new (InjectorItemType.Scope, parameter.ToLambda(parameter));

                        fields.AutoInjectMembers.Add(identifier, injectorItem);

                        break;
                    }
                    default:
                    {
                        throw new UnreachableException(member.ToString());
                    }
                }
            }

            if (
                injectorItem.Expression is LambdaExpression lambdaExpression
                && lambdaExpression.Parameters.IsSingle()
                && lambdaExpression.Body is ParameterExpression parameterExpression
                && lambdaExpression.Parameters[0].Type == parameterExpression.Type
            )
            {
                if (fields.Injectors.TryGetValue(parameterExpression.Type, out var injector))
                {
                    injectorItem = injector;
                }
                else
                {
                    variables.Add(parameterExpression);
                    memberExpressions.Add((member, parameterExpression));
                    isFull = false;

                    continue;
                }
            }

            if (
                !BuildExpression(
                    injectorItem.Expression.Type,
                    injectorItem,
                    scopeParameters,
                    out var expression
                )
            )
            {
                variables.AddRange(GetParameters(expression));
                isFull = false;
            }

            memberExpressions.Add((member, expression));
        }

        fields.CheckedInjectAttribute.TryAdd(root.Type);

        if (memberExpressions.IsEmpty())
        {
            result = root;

            return isFull;
        }

        var blockItems = new List<Expression>();
        var rootVariable = root.Type.ToVariableAutoName();
        blockItems.Add(rootVariable.ToAssign(root));
        variables.Add(rootVariable);

        foreach (var memberExpression in memberExpressions)
        {
            var assign = rootVariable
                .ToMember(memberExpression.Member)
                .ToAssign(memberExpression.Expression);

            blockItems.Add(assign);
        }

        blockItems.Add(rootVariable);
        result = variables.ToBlock(blockItems);

        return isFull;
    }

    private IEnumerable<ParameterExpression> GetParameters(Expression expression)
    {
        switch (expression)
        {
            case InvocationExpression invocationExpression:
            {
                foreach (var argument in invocationExpression.Arguments)
                foreach (var parameter in GetParameters(argument))
                {
                    yield return parameter;
                }

                break;
            }
            case ParameterExpression parameterExpression:
            {
                yield return parameterExpression;

                break;
            }
            case LambdaExpression lambdaExpression:
            {
                foreach (var parameter in lambdaExpression.Parameters)
                {
                    yield return parameter;
                }

                break;
            }
            case NewExpression newExpression:
            {
                foreach (var argument in newExpression.Arguments)
                foreach (var parameter in GetParameters(argument))
                {
                    yield return parameter;
                }

                break;
            }
            case MethodCallExpression methodCallExpression:
            {
                foreach (var argument in methodCallExpression.Arguments)
                foreach (var parameter in GetParameters(argument))
                {
                    yield return parameter;
                }

                if (methodCallExpression.Object is not null)
                {
                    foreach (var parameter in GetParameters(methodCallExpression.Object))
                    {
                        yield return parameter;
                    }
                }

                break;
            }
            case ConstantExpression:
            {
                break;
            }
            default:
            {
                var type = expression.GetType();

                throw new UnreachableException(type.ToString());
            }
        }
    }

    private Expression CreateParameter(
        ParameterExpression parameterExpression,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        if (!fields.Injectors.TryGetValue(parameterExpression.Type, out var injectorItem))
        {
            return parameterExpression;
        }

        BuildExpression(parameterExpression.Type, injectorItem, scopeParameters, out var result);

        return result;
    }

    private Func<object> BuildFunc(Expression expression)
    {
        if (expression.Type.IsValueType)
        {
            expression = expression.ToConvert(typeof(object));
        }

        var result = expression.ToLambda().Compile().ThrowIfIsNot<Func<object>>();

        return result;
    }

    private DependencyStatus GetLazyStatus(
        TypeInformation type,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        var instanceType = type.GenericTypeArguments.GetSingle();

        if (!fields.Injectors.TryGetValue(instanceType, out var injectorItem))
        {
            throw new TypeNotRegisterException(instanceType.Type);
        }

        BuildExpression(type, injectorItem, scopeParameters, out var expression);
        var options = fields.LazyOptions.GetValue(instanceType, LazyDependencyInjectorOptions.None);

        switch (options)
        {
            case LazyDependencyInjectorOptions.None:
            {
                var constructor = type.Type
                    .GetConstructor(
                        new[]
                        {
                            typeof(Func<>).MakeGenericType(instanceType.Type), typeof(LazyThreadSafetyMode)
                        }
                    )
                    .ThrowIfNull();

                return new (
                    type,
                    constructor.ToNew(expression.ToLambda(), LazyThreadSafetyMode.None.ToConstant())
                );
            }
            case LazyDependencyInjectorOptions.PublicationOnly:
            {
                var constructor = type.Type
                    .GetConstructor(
                        new[]
                        {
                            typeof(Func<>).MakeGenericType(instanceType.Type), typeof(LazyThreadSafetyMode)
                        }
                    )
                    .ThrowIfNull();

                return new (
                    type,
                    constructor.ToNew(
                        expression.ToLambda(),
                        LazyThreadSafetyMode.PublicationOnly.ToConstant()
                    )
                );
            }
            case LazyDependencyInjectorOptions.ExecutionAndPublication:
            {
                var constructor = type.Type
                    .GetConstructor(
                        new[]
                        {
                            typeof(Func<>).MakeGenericType(instanceType.Type), typeof(LazyThreadSafetyMode)
                        }
                    )
                    .ThrowIfNull();

                return new (
                    type,
                    constructor.ToNew(
                        expression.ToLambda(),
                        LazyThreadSafetyMode.ExecutionAndPublication.ToConstant()
                    )
                );
            }
            case LazyDependencyInjectorOptions.ThreadSafe:
            {
                var constructor = type.Type
                    .GetConstructor(
                        new[]
                        {
                            typeof(Func<>).MakeGenericType(instanceType.Type), typeof(bool)
                        }
                    )
                    .ThrowIfNull();

                return new (
                    type,
                    constructor.ToNew(expression.ToLambda(), true.ToConstant())
                );
            }
            default:
                throw new UnreachableException();
        }
    }

    private bool IsLazy(TypeInformation type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        if (type.Type.GetGenericTypeDefinition() == typeof(Lazy<>))
        {
            return true;
        }

        return false;
    }

    #region Checks

    private void Check(IReadOnlyDictionary<TypeInformation, InjectorItem> injectors)
    {
        CheckInjectors(injectors);
    }

    private void CheckInjectors(IReadOnlyDictionary<TypeInformation, InjectorItem> injectors)
    {
        var listParameterTypes = new List<TypeInformation>();

        foreach (var injector in injectors)
        {
            var keyType = injector.Key.Type;
            var injectorType = injector.Value.Expression.Type;
            var isAssignableFrom = injector.Value.Expression.Type.IsAssignableFrom(keyType);

            if (keyType != injectorType && isAssignableFrom)
            {
                throw new NotCovertException(injector.Key.Type, injector.Value.Expression.Type);
            }

            var types = GetParameters(injector.Value.Expression)
                .Select(x => (TypeInformation)x.Type);

            listParameterTypes.AddRange(types);

            if (listParameterTypes.Contains(injector.Key.Type))
            {
                throw new RecursionTypeExpressionInvokeException(
                    injector.Key.Type,
                    injector.Value.Expression
                );
            }

            listParameterTypes.Clear();
        }
    }

    #endregion
}