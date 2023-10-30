using System.Windows.Input;

namespace Spravy.Ui.Interfaces;

public interface IChangeFavoriteToDoItemCommand
{
    ICommand AddSubToDoItemToFavoriteCommand { get; }
    ICommand RemoveSubToDoItemFromFavoriteCommand { get; }
}