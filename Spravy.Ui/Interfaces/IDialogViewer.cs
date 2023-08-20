using System;
using System.Threading.Tasks;

namespace Spravy.Ui.Interfaces;

public interface IDialogViewer
{
    Task<object?> ShowDialogAsync(Type contentType);
    Task<object?> ShowDialogAsync<TView>(Action<TView>? setup = null) where TView : notnull;
    void CloseDialog();

    Task<object?> ShowConfirmDialogAsync<TView>(
        Func<TView, Task> cancelTask,
        Func<TView, Task> confirmTask,
        Action<TView>? setup = null
    ) where TView : notnull;
}