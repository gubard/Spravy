namespace Spravy.Domain.Interfaces;

public interface IHumanizing<in TInput, out TOutput>
    where TInput : notnull
{
    TOutput Humanize(TInput input);
}
