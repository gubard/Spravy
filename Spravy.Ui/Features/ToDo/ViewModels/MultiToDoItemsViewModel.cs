using Spravy.Ui.Features.ToDo.Views;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public partial class MultiToDoItemsViewModel : ViewModelBase
{
    [ObservableProperty]
    private GroupBy groupBy;

    [ObservableProperty]
    private ToDoItemViewType toDoItemViewType;

    [ObservableProperty]
    private bool isMulti;

    public MultiToDoItemsViewModel(ToDoItemsViewModel favorite, ToDoItemsGroupByViewModel toDoItems)
    {
        GroupBy = GroupBy.ByStatus;
        favorite.Header = new("MultiToDoItemsView.Favorite");
        Favorite = favorite;
        ToDoItems = toDoItems;
        PropertyChanged += OnPropertyChanged;
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
        return Favorite.AddOrUpdateUi(item);
    }

    public Result AddOrUpdateUi(ToDoItemEntityNotify item)
    {
        return ToDoItems.AddOrUpdateUi(item);
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
