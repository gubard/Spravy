namespace Spravy.Ui.Interfaces;

public interface INavigatable : IStateHolder, IRefresh
{
    bool IsPooled { get; }

    Result Stop();
}
