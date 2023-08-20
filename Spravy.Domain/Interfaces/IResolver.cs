using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IResolver
{
    object Resolve(TypeInformation type);
}