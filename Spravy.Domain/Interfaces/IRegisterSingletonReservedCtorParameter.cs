using System.Linq.Expressions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IRegisterSingletonReservedCtorParameter
{
    void RegisterSingletonReservedCtorParameter(
        ReservedCtorParameterIdentifier identifier,
        Expression expression
    );
}