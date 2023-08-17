using System.Threading.Tasks;
using ExtensionFramework.AvaloniaUi.Extensions;
using ExtensionFramework.AvaloniaUi.Models;
using Spravy.ToDo.Domain.Models;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.Extensions;

public static class ToDoItemOrderChangerExtension
{
    public static async Task ChangeToDoItemOrderAsync(
        this IToDoItemOrderChanger changer,
        MovedDragDropArgs<ToDoSubItemNotify> args
    )
    {
        if (changer.ToDoService is null)
        {
            return;
        }

        if (args.TargetIndex == args.SourceIndex)
        {
            return;
        }
        
        var id = args.SourceItem.Id;
        var isAfter = GetIsAfter(args);
        var options = new UpdateOrderIndexToDoItemOptions(id, args.TargetItem.Id, isAfter);
        await changer.ToDoService.UpdateToDoItemOrderIndexAsync(options);
        await changer.RefreshToDoItemAsync();
    }

    private static bool GetIsAfter(MovedDragDropArgs<ToDoSubItemNotify> args)
    {
        if (args.TargetIndex == args.SourceIndex + 1)
        {
            return true;
        }
        
        if (args.TargetIndex == args.SourceIndex - 1)
        {
            return false;
        }
        
        return args.Item.Bounds.IsBottom(args.PointerPosition);
    }
}