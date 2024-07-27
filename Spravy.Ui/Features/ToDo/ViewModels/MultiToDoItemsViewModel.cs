using Avalonia.Markup.Xaml.Styling;
using Spravy.Ui.Features.ToDo.Views;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class MultiToDoItemsViewModel : ViewModelBase
{
    public MultiToDoItemsViewModel(ToDoItemsViewModel favorite, ToDoItemsGroupByViewModel toDoItems)
    {
        GroupBy = GroupBy.ByStatus;
        favorite.Header = new("MultiToDoItemsView.Favorite");
        Favorite = favorite;
        ToDoItems = toDoItems;

        this.WhenAnyValue(x => x.GroupBy).Subscribe(x => ToDoItems.GroupBy = x);

        this.WhenAnyValue(x => x.IsMulti)
            .Subscribe(x =>
            {
                Favorite.IsMulti = x;
                ToDoItems.IsMulti = x;
            });
    }

    public ToDoItemsViewModel Favorite { get; }
    public ToDoItemsGroupByViewModel ToDoItems { get; }
    public MultiToDoItemsView? MultiToDoItemsView { get; set; }

    [Reactive]
    public GroupBy GroupBy { get; set; }

    [Reactive]
    public ToDoItemViewType ToDoItemViewType { get; set; }

    [Reactive]
    public bool IsMulti { get; set; }

    public Result ClearFavoriteExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return Favorite.ClearExceptUi(ids);
    }

    public Result ClearExceptUi(ReadOnlyMemory<ToDoItemEntityNotify> ids)
    {
        return ToDoItems.ClearExceptUi(ids);
    }

    public Result UpdateFavoriteItemUi(ToDoItemEntityNotify item)
    {
        return Favorite.UpdateItemUi(item);
    }

    public Result UpdateItemUi(ToDoItemEntityNotify item)
    {
        return ToDoItems.UpdateItemUi(item);
    }
}
