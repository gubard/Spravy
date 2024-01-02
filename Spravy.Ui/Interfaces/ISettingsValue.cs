namespace Spravy.Ui.Interfaces;

public interface ISettingsValue<out TValue>
{
    static abstract TValue Default { get; }
}