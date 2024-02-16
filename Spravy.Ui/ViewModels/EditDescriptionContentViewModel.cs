using System;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class EditDescriptionContentViewModel : ViewModelBase
{
    private string description = string.Empty;
    private DescriptionType type;

    public AvaloniaList<DescriptionType> Types { get; } = new(Enum.GetValues<DescriptionType>());

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public DescriptionType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }
}