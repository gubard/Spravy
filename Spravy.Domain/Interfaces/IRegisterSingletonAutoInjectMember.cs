using System.Linq.Expressions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IRegisterSingletonAutoInjectMember
{
    void RegisterSingletonAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    );
}