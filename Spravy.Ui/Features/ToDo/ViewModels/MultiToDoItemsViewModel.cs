using Spravy.Ui.Features.ToDo.Views;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemsViewModel : ViewModelBase
{
    [ObservableProperty]
    public GroupBy groupBy;

    [ObservableProperty]
    public ToDoItemViewType toDoItemViewType;

    [ObservableProperty]
    public bool isMulti;

    public MultiToDoItemsViewModel(ToDoItemsViewModel favorite, ToDoItemsGroupByViewModel toDoItems)
    {
        GroupBy = GroupBy.ByStatus;
        favorite.Header = new("MultiToDoItemsView.Favorite");
        Favorite = favorite;
        ToDoItems = toDoItems;
    }

    public ToDoItemsViewModel Favorite { get; }
    public ToDoItemsGroupByViewModel ToDoItems { get; }
    public MultiToDoItemsView? MultiToDoItemsView { get; set; }

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

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GroupBy):
                ToDoItems.GroupBy = GroupBy;

                break;
            case nameof(ToDoItemViewType) when MultiToDoItemsView is null:
                return;
            case nameof(ToDoItemViewType):
            {
                if (!MultiToDoItemsView.TryGetStyle("CardToDoItems", out var style))
                {
                    return;
                }

                switch (ToDoItemViewType)
                {
                    case ToDoItemViewType.List:
                        MultiToDoItemsView.Styles.Remove(style);
                        break;
                    case ToDoItemViewType.Card:
                        if (!MultiToDoItemsView.Styles.Contains(style))
                        {
                            MultiToDoItemsView.Styles.Add(style);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(ToDoItemViewType),
                            ToDoItemViewType,
                            null
                        );
                }

                break;
            }
        }
    }
}
