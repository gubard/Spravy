namespace Spravy.ToDo.Domain.Models;

public readonly struct GetToDo
{
    public GetToDo(
        bool isSelectorItems,
        ReadOnlyMemory<GetToStringItem> toStringItems,
        bool isCurrentActiveItem,
        ReadOnlyMemory<Guid> activeItems,
        bool isFavoriteItems,
        bool isBookmarkItems,
        ReadOnlyMemory<Guid> childrenItems,
        ReadOnlyMemory<Guid> leafItems,
        string searchText,
        ReadOnlyMemory<Guid> parentItems,
        bool isTodayItems,
        bool isRootItems,
        ReadOnlyMemory<Guid> items
    )
    {
        IsSelectorItems = isSelectorItems;
        ToStringItems = toStringItems;
        IsCurrentActiveItem = isCurrentActiveItem;
        ActiveItems = activeItems;
        IsFavoriteItems = isFavoriteItems;
        IsBookmarkItems = isBookmarkItems;
        ChildrenItems = childrenItems;
        LeafItems = leafItems;
        SearchText = searchText;
        ParentItems = parentItems;
        IsTodayItems = isTodayItems;
        IsRootItems = isRootItems;
        Items = items;
    }

    public readonly bool IsSelectorItems;
    public readonly ReadOnlyMemory<GetToStringItem> ToStringItems;
    public readonly bool IsCurrentActiveItem;
    public readonly ReadOnlyMemory<Guid> ActiveItems;
    public readonly bool IsFavoriteItems;
    public readonly bool IsBookmarkItems;
    public readonly ReadOnlyMemory<Guid> ChildrenItems;
    public readonly ReadOnlyMemory<Guid> LeafItems;
    public readonly string SearchText;
    public readonly ReadOnlyMemory<Guid> ParentItems;
    public readonly bool IsTodayItems;
    public readonly bool IsRootItems;
    public readonly ReadOnlyMemory<Guid> Items;
}