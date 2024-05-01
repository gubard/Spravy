namespace Spravy.Ui.Models;

public class MovedDragDropArgs<TItem>
{
    public MovedDragDropArgs(
        TItem sourceItem,
        int sourceIndex,
        TItem targetItem,
        int targetIndex,
        ListBoxItem item,
        Point pointerPosition
    )
    {
        SourceItem = sourceItem;
        SourceIndex = sourceIndex;
        TargetItem = targetItem;
        TargetIndex = targetIndex;
        Item = item;
        PointerPosition = pointerPosition;
    }

    public TItem SourceItem { get; }
    public int SourceIndex { get; }
    public TItem TargetItem { get; }
    public int TargetIndex { get; }
    public ListBoxItem Item { get; }
    public Point PointerPosition { get; }
}