using System.Linq.Expressions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IExpressionCache
{
    void CacheExpression(TypeInformation type);
    Expression? GetCacheExpression(TypeInformation type);
}