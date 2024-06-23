namespace Spravy.Domain.Interfaces;

public interface IServiceFactory
{
    T CreateService<T>() where T : notnull;
}