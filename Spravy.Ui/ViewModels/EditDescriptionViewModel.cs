using System;
using Avalonia.Collections;
using ReactiveUI;
using Spravy.Domain.Enums;
using Spravy.Ui.Models;

namespace Spravy.Ui.ViewModels;

public class EditDescriptionViewModel : ViewModelBase
{
    private string toDoItemName = string.Empty;
    private string description = string.Empty;
    private bool isPlainText;
    private bool isMarkdown;

    public bool IsMarkdown
    {
        get => isMarkdown;
        set => this.RaiseAndSetIfChanged(ref isMarkdown, value);
    }

    public bool IsPlainText
    {
        get => isPlainText;
        set => this.RaiseAndSetIfChanged(ref isPlainText, value);
    }

    public string Description
    {
        get => description;
        set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public string ToDoItemName
    {
        get => toDoItemName;
        set => this.RaiseAndSetIfChanged(ref toDoItemName, value);
    }

    public DescriptionType Type
    {
        get => IsPlainText ? DescriptionType.PlainText : DescriptionType.Markdown;
        set
        {
            switch (value)
            {
                case DescriptionType.PlainText:
                    IsPlainText = true;
                    break;
                case DescriptionType.Markdown:
                    IsMarkdown = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}