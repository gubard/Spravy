using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoResponse
{
    public ToDoResponse(
        ReadOnlyMemory<ToDoSelectorItem> selectorItems,
        ReadOnlyMemory<ToStringItem> toStringItems,
        OptionStruct<ToDoShortItem> currentActive,
        ReadOnlyMemory<ActiveItem> activeItems,
        ReadOnlyMemory<FullToDoItem> favoriteItems,
        ReadOnlyMemory<ToDoShortItem> bookmarkItems,
        ReadOnlyMemory<ChildrenItem> childrenItems,
        ReadOnlyMemory<LeafItem> leafItems,
        ReadOnlyMemory<FullToDoItem> searchItems,
        ReadOnlyMemory<ParentItem> parentItems,
        ReadOnlyMemory<FullToDoItem> todayItems,
        ReadOnlyMemory<FullToDoItem> rootItems,
        ReadOnlyMemory<FullToDoItem> items
    )
    {
        SelectorItems = selectorItems;
        ToStringItems = toStringItems;
        CurrentActive = currentActive;
        ActiveItems = activeItems;
        FavoriteItems = favoriteItems;
        BookmarkItems = bookmarkItems;
        ChildrenItems = childrenItems;
        LeafItems = leafItems;
        SearchItems = searchItems;
        ParentItems = parentItems;
        TodayItems = todayItems;
        RootItems = rootItems;
        Items = items;
    }

    public readonly ReadOnlyMemory<ToDoSelectorItem> SelectorItems;
    public readonly ReadOnlyMemory<ToStringItem> ToStringItems;
    public readonly OptionStruct<ToDoShortItem> CurrentActive;
    public readonly ReadOnlyMemory<ActiveItem> ActiveItems;
    public readonly ReadOnlyMemory<FullToDoItem> FavoriteItems;
    public readonly ReadOnlyMemory<ToDoShortItem> BookmarkItems;
    public readonly ReadOnlyMemory<ChildrenItem> ChildrenItems;
    public readonly ReadOnlyMemory<LeafItem> LeafItems;
    public readonly ReadOnlyMemory<FullToDoItem> SearchItems;
    public readonly ReadOnlyMemory<ParentItem> ParentItems;
    public readonly ReadOnlyMemory<FullToDoItem> TodayItems;
    public readonly ReadOnlyMemory<FullToDoItem> RootItems;
    public readonly ReadOnlyMemory<FullToDoItem> Items;
}