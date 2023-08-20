namespace Spravy.Domain.Interfaces;

public interface IDependencyInjectorRegister
    : IRegisterTransient,
        IRegisterSingleton,
        IRegisterAutoInjectMember,
        IRegisterConfiguration,
        IRegisterReservedCtorParameter,
        IRegisterScope,
        ILazyConfigurator
{
}