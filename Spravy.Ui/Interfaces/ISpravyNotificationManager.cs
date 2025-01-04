namespace Spravy.Ui.Interfaces;

public interface ISpravyNotificationManager
{
    Result Show<TView>() where TView : notnull;
    Result Show(object view);
}