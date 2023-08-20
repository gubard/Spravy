using System.Linq.Expressions;

namespace Spravy.Domain.Interfaces;

public interface IRegisterScope
{
    void RegisterScope(Type type, Expression expression);
}