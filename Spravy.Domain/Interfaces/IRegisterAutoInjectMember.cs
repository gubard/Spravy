namespace Spravy.Domain.Interfaces;

public interface IRegisterAutoInjectMember
    : IRegisterTransientAutoInjectMember,
        IRegisterSingletonAutoInjectMember,
        IRegisterScopeAutoInjectMember
{
}