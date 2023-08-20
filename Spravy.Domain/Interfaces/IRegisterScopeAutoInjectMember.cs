using System.Linq.Expressions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IRegisterScopeAutoInjectMember
{
    void RegisterScopeAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    );
}