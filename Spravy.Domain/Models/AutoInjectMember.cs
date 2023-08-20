using System.Diagnostics;
using System.Reflection;
using Spravy.Domain.Extensions;

namespace Spravy.Domain.Models;

public readonly record struct AutoInjectMember
{
    private readonly string name;

    public AutoInjectMember(MemberInfo member)
    {
        name = member.Name;

        switch (member)
        {
            case FieldInfo field:
            {
                Type = field.FieldType;

                break;
            }
            case PropertyInfo property:
            {
                Type = property.PropertyType;

                break;
            }
            case MethodInfo method:
            {
                Type = method.ReturnType;

                break;
            }
            case ConstructorInfo constructor:
            {
                Type = constructor.DeclaringType.ThrowIfNull();

                break;
            }
            case EventInfo evn:
            {
                Type = evn.EventHandlerType.ThrowIfNull();

                break;
            }
            case TypeInfo type:
            {
                Type = type;

                break;
            }
            default:
            {
                var type = member.GetType();

                throw new UnreachableException(type.ToString());
            }
        }
    }

    public TypeInformation Type { get; }

    public static implicit operator AutoInjectMember(MemberInfo member)
    {
        return new (member);
    }
}