using System.Collections.Generic;

namespace _build.Extensions;

public static class ListExtension
{
    public static TItem AddItem<TItem, TListItem>(this List<TListItem> list, TItem item)
        where TItem : TListItem
    {
        list.Add(item);

        return item;
    }
}
