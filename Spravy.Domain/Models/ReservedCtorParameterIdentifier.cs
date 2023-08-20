using System.Reflection;

namespace Spravy.Domain.Models;

public readonly record struct ReservedCtorParameterIdentifier(
    TypeInformation Type,
    ConstructorInfo Constructor,
    ParameterInfo Parameter
);