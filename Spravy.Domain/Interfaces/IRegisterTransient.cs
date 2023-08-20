using System.Linq.Expressions;

namespace Spravy.Domain.Interfaces;

public interface IRegisterTransient
{
    void RegisterTransient(Type type, Expression expression);
}