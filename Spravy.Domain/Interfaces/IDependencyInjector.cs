using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IDependencyInjector : IResolver, IInvoker, IDependencyStatusGetter
{
    ReadOnlyMemory<TypeInformation> Inputs { get; }
    ReadOnlyMemory<TypeInformation> Outputs { get; }
}