using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Spravy.Domain.Exceptions;
using Spravy.Domain.Extensions;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;

namespace Spravy.Domain.Services;

public class ModuleTree : IModule, IResolver, IInvoker
{
    private readonly Dictionary<TypeInformation, Func<object>> cache;
    private readonly Expression thisExpression;
    private readonly Tree<Guid, IModule> tree;

    public ModuleTree(Tree<Guid, IModule> tree)
    {
        this.tree = tree;
        cache = new();
        Id = Guid.NewGuid();
        var inputs = new List<TypeInformation>();
        var outputs = new List<TypeInformation>();
        var ends = this.tree.GetEnds();

        foreach (var node in ends)
        {
            AddTypes(inputs, outputs, node);
        }

        var outputsArray = outputs
            .Distinct()
            .Concat(
                new TypeInformation[]
                {
                    typeof(IResolver), typeof(IInvoker)
                }
            )
            .OrderBy(x => x.ToString())
            .ToArray();

        Outputs = outputsArray;
        Inputs = inputs.Distinct().Where(x => !outputsArray.Contains(x)).ToArray();
        thisExpression = this.ToConstant();
    }

    public bool IsFull => Inputs.IsEmpty;

    public object? Invoke(Delegate del, DictionarySpan<TypeInformation, object> arguments)
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

    public object? Invoke(object? obj, MethodInfo method, DictionarySpan<TypeInformation, object> arguments)
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

    public Guid Id { get; }
    public ReadOnlyMemory<TypeInformation> Inputs { get; }
    public ReadOnlyMemory<TypeInformation> Outputs { get; }

    public DependencyStatus GetStatus(
        TypeInformation type,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        if (typeof(IResolver) == type)
        {
            return new (type, thisExpression);
        }

        if (typeof(IInvoker) == type)
        {
            return new (type, thisExpression);
        }

        if (!IsTypeContains(type))
        {
            throw new TypeNotRegisterException(type.Type);
        }

        if (IsTypeContains((ReadOnlyMemory<TypeInformation>)tree.Root.Value.Outputs, type))
        {
            var rootStatus = tree.Root.Value.GetStatus(type, scopeParameters);

            return rootStatus;
        }

        var status = GetDependencyStatus(tree.Root, type, scopeParameters).ThrowIfNullStruct();

        return status;
    }

    public object GetObject(TypeInformation type)
    {
        if (typeof(IResolver) == type)
        {
            return this;
        }

        if (typeof(IInvoker) == type)
        {
            return this;
        }

        if (!IsTypeContains(type))
        {
            throw new TypeNotRegisterException(type.Type);
        }

        if (cache.TryGetValue(type, out var func))
        {
            var value = func.Invoke();

            return value;
        }

        var scopeParameters = new Dictionary<TypeInformation, ScopeValue>();
        var status = GetStatus(type, scopeParameters);
        var expression = UpdateExpression(status.Expression, scopeParameters);

        if (expression.Type.IsValueType)
        {
            expression = expression.ToConvert(typeof(object));
        }

        var lambda = InitScopeValues(expression, scopeParameters).ToLambda();
        func = lambda.Compile().ThrowIfIsNot<Func<object>>();

        try
        {
            var result = func.Invoke();
            cache.Add(type, func);

            return result;
        }
        catch (Exception e)
        {
            throw new (lambda.ToString(), e);
        }
    }

    public object Resolve(TypeInformation type)
    {
        return GetObject(type);
    }

    public void SetupModule()
    {
        var moduleSetup = this.Resolve<IModuleSetup>();
        var setupMethod = moduleSetup.GetType().GetMethod("Setup");
        this.Invoke(moduleSetup, setupMethod);
    }

    private Expression InitScopeValues(
        Expression expression,
        Dictionary<TypeInformation, ScopeValue> values
    )
    {
        if (values.IsEmpty())
        {
            return expression;
        }

        for (var index = 0; index < values.Count; index++)
        {
            var keys = values.Keys.ToArray();
            var key = keys[index];
            var exp = UpdateExpression(values[key].Expression, values);

            values[key] = values[key] with
            {
                Expression = exp
            };
        }

        var blockItems = new List<Expression>();
        var variables = values.Select(x => x.Value.Parameter).Distinct().ToArray();
        var parameters = values.GroupBy(x => x.Value.Parameter);

        foreach (var parameter in parameters)
        {
            blockItems.Add(parameter.Key.ToAssign(parameter.First().Value.Expression));
        }

        blockItems.Add(expression);

        return variables.ToBlock(blockItems);
    }

    private bool IsTypeContains(ReadOnlyMemory<TypeInformation> outputs, TypeInformation type)
    {
        if (!outputs.Span.Contains(type))
        {
            if (!type.Type.IsGenericType)
            {
                return false;
            }

            if (type.Type.GetGenericTypeDefinition() != typeof(Lazy<>))
            {
                return false;
            }

            var argument = type.Type.GenericTypeArguments.Single();

            if (!outputs.Span.Contains(argument))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsTypeContains(TypeInformation type)
    {
        return IsTypeContains(Outputs, type);
    }

    private bool IsTypeContains(TreeNode<Guid, IModule> node, TypeInformation type)
    {
        return IsTypeContains((ReadOnlyMemory<TypeInformation>)node.Value.Outputs, type);
    }

    private Expression UpdateExpression(
        Expression expression,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        switch (expression)
        {
            case InvocationExpression invocationExpression:
            {
                var arguments = new List<Expression>();

                foreach (var argument in invocationExpression.Arguments)
                {
                    arguments.Add(UpdateExpression(argument, scopeParameters));
                }

                var result = invocationExpression.Update(
                    invocationExpression.Expression,
                    arguments
                );

                return result;
            }
            case ParameterExpression parameterExpression:
            {
                if (scopeParameters.TryGetValue(parameterExpression.Type, out var value))
                {
                    return value.Parameter;
                }

                var result = CreateParameter(parameterExpression, scopeParameters);

                return result;
            }
            case NewExpression newExpression:
            {
                var arguments = new List<Expression>();

                foreach (var argument in newExpression.Arguments)
                {
                    var argumentExpression = UpdateExpression(argument, scopeParameters);

                    if (argumentExpression.Type.IsValueType)
                    {
                        argumentExpression = argumentExpression.ToConvert(argument.Type);
                    }

                    arguments.Add(argumentExpression);
                }

                return newExpression.Update(arguments);
            }
            case BlockExpression blockExpression:
            {
                var expressions = new List<Expression>();
                var blockExpressionItems = blockExpression.Expressions.Take(
                    blockExpression.Expressions.Count - 1
                );
                var blockResult = blockExpression.Expressions
                    .Last()
                    .ThrowIfIsNot<ParameterExpression>();

                foreach (var blockExpressionItem in blockExpressionItems)
                {
                    expressions.Add(UpdateExpression(blockExpressionItem, scopeParameters));
                }

                expressions.Add(blockResult);
                var result = blockExpression.Update(blockResult.AsArray(), expressions);

                return result;
            }
            case ConstantExpression constantExpression:
            {
                return constantExpression;
            }
            case BinaryExpression binaryExpression:
            {
                var conversion = binaryExpression.Conversion is null
                    ? null
                    : UpdateExpression(binaryExpression, scopeParameters)
                        .ThrowIfIsNot<LambdaExpression>();
                var right = UpdateExpression(binaryExpression.Right, scopeParameters);
                var result = binaryExpression.Update(binaryExpression.Left, conversion, right);

                return result;
            }
            case MethodCallExpression methodCallExpression:
            {
                var obj = methodCallExpression.Object is null
                    ? null
                    : UpdateExpression(methodCallExpression.Object, scopeParameters);
                var arguments = new List<Expression>();

                foreach (var argument in methodCallExpression.Arguments)
                {
                    arguments.Add(UpdateExpression(argument, scopeParameters));
                }

                return methodCallExpression.Update(obj, arguments);
            }
            case LambdaExpression lambdaExpression:
            {
                var body = UpdateExpression(lambdaExpression.Body, scopeParameters);
                var expressions = new List<Expression>();

                foreach (var parameter in lambdaExpression.Parameters)
                {
                    expressions.Add(UpdateExpression(parameter, scopeParameters));
                }

                if (expressions.Any())
                {
                    var result = body.ToLambda().ToInvoke(expressions).ToLambda();

                    return result;
                }
                else
                {
                    var result = body.ToLambda();

                    return result;
                }
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
        var status = GetStatus(parameterExpression.Type, scopeParameters);
        var expression = UpdateExpression(status.Expression, scopeParameters);

        return expression;
    }

    private DependencyStatus? GetDependencyStatus(
        TreeNode<Guid, IModule> node,
        TypeInformation type,
        Dictionary<TypeInformation, ScopeValue> scopeParameters
    )
    {
        if (typeof(IResolver) == type)
        {
            return new DependencyStatus(type, thisExpression);
        }

        if (typeof(IInvoker) == type)
        {
            return new DependencyStatus(type, thisExpression);
        }

        foreach (var treeNode in node.Nodes)
        {
            if (IsTypeContains(treeNode, type))
            {
                var status = treeNode.Value.GetStatus(type, scopeParameters);

                return status;
            }
        }

        foreach (var treeNode in node.Nodes)
        {
            var status = GetDependencyStatus(treeNode, type, scopeParameters);

            if (status is null)
            {
                continue;
            }

            return status;
        }

        throw new TypeNotRegisterException(type.Type);
    }

    private void AddTypes(
        List<TypeInformation> inputs,
        List<TypeInformation> outputs,
        TreeNode<Guid, IModule> node
    )
    {
        outputs.AddRange(node.Value.Outputs.ToArray());
        inputs.AddRange(node.Value.Inputs.ToArray());

        if (node.Parent is null)
        {
            return;
        }

        AddTypes(inputs, outputs, node.Parent);
    }
}