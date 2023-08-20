using System.Linq.Expressions;

namespace Spravy.Domain.Interfaces;

public interface IRegisterSingleton
{
    void RegisterSingleton(Type type, Expression expression);
}