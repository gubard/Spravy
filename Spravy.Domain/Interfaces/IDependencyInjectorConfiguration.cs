namespace Spravy.Domain.Interfaces;

public interface IDependencyInjectorConfiguration
{
    void Configure(IDependencyInjectorRegister register);
}