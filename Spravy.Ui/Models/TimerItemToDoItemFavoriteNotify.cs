using ReactiveUI;

namespace Spravy.Ui.Models;

public class TimerItemToDoItemFavoriteNotify : TimerItemNotify
{
    private bool isFavorite;

    public bool IsFavorite
    {
        get => isFavorite;
        set => this.RaiseAndSetIfChanged(ref isFavorite, value);
    }
}