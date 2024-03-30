using Spravy.Domain.Models;

namespace Spravy.Domain.Interfaces;

public interface IConverter
{
    Result<TResult> Convert<TResult>(object source);
}