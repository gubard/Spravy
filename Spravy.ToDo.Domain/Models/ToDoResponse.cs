using Spravy.Domain.Models;

namespace Spravy.ToDo.Domain.Models;

public readonly struct ToDoResponse
{
    public ToDoResponse(
        ToDoSelectorItemsResponse selectorItems,
        ReadOnlyMemory<ToStringItem> toStringItems,
        OptionStruct<ToDoShortItem> currentActive,
        ReadOnlyMemory<ActiveItem> activeItems,
        ToDoFullItemsResponse favoriteItems,
        ToDoShortItemsResponse bookmarkItems,
        ReadOnlyMemory<ChildrenItem> childrenItems,
        ReadOnlyMemory<LeafItem> leafItems,
        ToDoFullItemsResponse searchItems,
        ReadOnlyMemory<ParentItem> parentItems,
        ToDoFullItemsResponse todayItems,
        ToDoFullItemsResponse rootItems,
        ToDoFullItemsResponse items
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

    public readonly ToDoSelectorItemsResponse SelectorItems;
    public readonly ReadOnlyMemory<ToStringItem> ToStringItems;
    public readonly OptionStruct<ToDoShortItem> CurrentActive;
    public readonly ReadOnlyMemory<ActiveItem> ActiveItems;
    public readonly ToDoFullItemsResponse FavoriteItems;
    public readonly ToDoShortItemsResponse BookmarkItems;
    public readonly ReadOnlyMemory<ChildrenItem> ChildrenItems;
    public readonly ReadOnlyMemory<LeafItem> LeafItems;
    public readonly ToDoFullItemsResponse SearchItems;
    public readonly ReadOnlyMemory<ParentItem> ParentItems;
    public readonly ToDoFullItemsResponse TodayItems;
    public readonly ToDoFullItemsResponse RootItems;
    public readonly ToDoFullItemsResponse Items;
}