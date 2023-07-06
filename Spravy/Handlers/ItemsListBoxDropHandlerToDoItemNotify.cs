using ExtensionFramework.AvaloniaUi.Handlers;
using Spravy.Extensions;
using Spravy.Interfaces;
using Spravy.Models;

namespace Spravy.Handlers;

public class ItemsListBoxDropHandlerToDoItemNotify : ItemsListBoxDropHandler<ToDoItemNotify>
{
    public ItemsListBoxDropHandlerToDoItemNotify()
    {
        Moved += async (s, a) =>
        {
            if (s is not IToDoItemOrderChanger changer)
            {
                return;
            }

            await changer.ChangeToDoItemOrderAsync(a);
        };
    }
}