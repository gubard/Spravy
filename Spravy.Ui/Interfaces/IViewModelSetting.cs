namespace Spravy.Ui.Interfaces;

public interface IViewModelSetting<out T>
{
    static abstract T Default { get; }
}