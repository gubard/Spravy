using System.Linq.Expressions;
using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IRegisterTransientAutoInjectMember
{
    void RegisterTransientAutoInjectMember(
        AutoInjectMemberIdentifier memberIdentifier,
        Expression expression
    );
}