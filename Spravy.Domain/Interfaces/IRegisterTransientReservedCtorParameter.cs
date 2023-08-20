using System.Linq.Expressions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IRegisterTransientReservedCtorParameter
{
    void RegisterTransientReservedCtorParameter(
        ReservedCtorParameterIdentifier identifier,
        Expression expression
    );
}