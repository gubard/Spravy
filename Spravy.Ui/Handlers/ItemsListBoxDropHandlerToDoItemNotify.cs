using ExtensionFramework.AvaloniaUi.Handlers;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
using Spravy.Ui.Models;

namespace Spravy.Ui.Handlers;

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