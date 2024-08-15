namespace Spravy.Ui.Interfaces;

public interface INavigatable : IStateHolder
{
    bool IsPooled { get; }
    string ViewId { get; }

    Result Stop();
}
