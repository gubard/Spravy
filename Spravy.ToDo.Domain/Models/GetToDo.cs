﻿using Spravy.ToDo.Domain.Enums;

namespace Spravy.ToDo.Domain.Models;

public readonly struct GetToDo
{
    public static GetToDo Default = new GetToDo().SetSearchText(new(string.Empty, ReadOnlyMemory<ToDoItemType>.Empty));
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
        GetSearch search,
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
        Search = search;
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
    public readonly GetSearch Search;
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
            Search,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetActiveItem(Guid value)
    {
        return SetActiveItems(
            new[]
            {
                value,
            }
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
            Search,
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
            Search,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetChildrenItem(Guid value)
    {
        return SetChildrenItems(
            new[]
            {
                value,
            }
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
            Search,
            ParentItems,
            value,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetSearchText(GetSearch value)
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
            Search,
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
            Search,
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
            Search,
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
            Search,
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
            Search,
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
            Search,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetToStringItem(GetToStringItem value)
    {
        return SetToStringItems(
            new[]
            {
                value,
            }
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
            Search,
            ParentItems,
            IsTodayItems,
            IsRootItems,
            value
        );
    }

    public GetToDo SetItem(Guid value)
    {
        return SetItems(
            new[]
            {
                value,
            }
        );
    }

    public GetToDo SetParentItems(ReadOnlyMemory<Guid> value)
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
            Search,
            value,
            IsTodayItems,
            IsRootItems,
            Items
        );
    }

    public GetToDo SetParentItem(Guid value)
    {
        return SetParentItems(
            new[]
            {
                value,
            }
        );
    }
}