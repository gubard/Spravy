using System;
using Avalonia.Collections;
using Ninject;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class EditDescriptionViewModel : ViewModelBase
{
    private string toDoItemName = string.Empty;

    public string ToDoItemName
    {
        get => toDoItemName;
        set => this.RaiseAndSetIfChanged(ref toDoItemName, value);
    }

    [Inject]
    public required EditDescriptionContentViewModel Content { get; init; }
}