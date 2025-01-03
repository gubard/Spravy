namespace Spravy.ToDo.Domain.Models;

public readonly struct GetToDo
{
    public static GetToDo Default = new GetToDo().SetSearchText(string.Empty);
    public static GetToDo WithDefaultItems = Default.SetIsBookmarkItems(true).SetIsFavoriteItems(true);

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
    public readonly string SearchText = string.Empty;
    public readonly ReadOnlyMemory<Guid> ParentItems;
    public readonly bool IsTodayItems;
    public readonly bool IsRootItems;
    public readonly ReadOnlyMemory<Guid> Items;
    
    public GetToDo SetActiveItems(ReadOnlyMemory<Guid> value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            value,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }
    
    public GetToDo SetIsCurrentActiveItem(bool value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            value,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetChildrenItems(ReadOnlyMemory<Guid> value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            value,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetIsTodayItems(bool value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            value,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetSearchText(string value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            value,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetIsRootItems(bool value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            value,
            Items
        );
    }

    public GetToDo SetLeafItems(ReadOnlyMemory<Guid> value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            value,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetIsSelectorItems(bool value)
    {
        return new(
            value,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetIsFavoriteItems(bool value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            value,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetIsBookmarkItems(bool value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            value,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetToStringItems(ReadOnlyMemory<GetToStringItem> value)
    {
        return new(
            IsSelectorItems,
            value,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetItems(ReadOnlyMemory<Guid> value)
    {
        return new(
            IsSelectorItems,
            ToStringItems,
            IsCurrentActiveItem,
            ActiveItems,
            IsFavoriteItems,
            IsBookmarkItems,
            ChildrenItems,
            LeafItems,
            SearchText,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            value
        );
    }
}