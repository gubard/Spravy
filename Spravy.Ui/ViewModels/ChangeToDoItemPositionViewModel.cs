using System;
using Avalonia.Collections;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class ChangeToDoItemPositionViewModel : ViewModelBase
{
    public Guid Id { get; set; }
    public AvaloniaList<ToDoItemNotify> Items { get; } = new();
}