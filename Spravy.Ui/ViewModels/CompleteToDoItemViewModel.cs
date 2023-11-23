using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.ToDo.Domain.Enums;
using Spravy.Ui.Enums;
using Spravy.Ui.Models;
using Spravy.Ui.Views;

namespace Spravy.Ui.ViewModels;

public class CompleteToDoItemViewModel : ViewModelBase
{
    private static readonly Dictionary<CompleteStatus, KeyGesture> keys = new()
    {
        {
            CompleteStatus.Complete, new(Key.C)
        },
        {
            CompleteStatus.Incomplete, new(Key.I)
        },
        {
            CompleteStatus.Fail, new(Key.F)
        },
        {
            CompleteStatus.Skip, new(Key.S)
        },
    };

    private CompleteToDoItemView? view;

    public CompleteToDoItemViewModel()
    {
        CompleteCommand = CreateCommandFromTask<CompleteStatus>(CompleteAsync);
        InitializedCommand = CreateInitializedCommand<CompleteToDoItemView>(Initialized);
    }

    public AvaloniaList<Ref<CompleteStatus>> CompleteStatuses { get; } = new();
    public ICommand CompleteCommand { get; }
    public ICommand InitializedCommand { get; }
    public Func<CompleteStatus, Task>? Complete { get; set; }

    private void Initialized(CompleteToDoItemView args)
    {
        view = args;
        UpdateKeyBindings();
    }

    private Task CompleteAsync(CompleteStatus status)
    {
        return Complete.ThrowIfNull().Invoke(status);
    }
    
    public void SetAllStatus()
    {
        CompleteStatuses.Clear();
        CompleteStatuses.AddRange(Enum.GetValues<CompleteStatus>().Select(x => new Ref<CompleteStatus>(x)));
        UpdateKeyBindings();
    }

    public void SetCompleteStatus(ToDoItemIsCan isCan)
    {
        CompleteStatuses.Clear();

        if (isCan.HasFlag(ToDoItemIsCan.CanComplete))
        {
            CompleteStatuses.Add(new (CompleteStatus.Complete));
        }
        
        if (isCan.HasFlag(ToDoItemIsCan.CanIncomplete))
        {
            CompleteStatuses.Add(new (CompleteStatus.Incomplete));
        }
        
        if (isCan.HasFlag(ToDoItemIsCan.CanSkip))
        {
            CompleteStatuses.Add(new (CompleteStatus.Skip));
        }
        
        if (isCan.HasFlag(ToDoItemIsCan.CanFail))
        {
            CompleteStatuses.Add(new (CompleteStatus.Fail));
        }

        UpdateKeyBindings();
    }
    
    private void UpdateKeyBindings()
    {
        if(view is null)
        {
            return;
        }

        view.KeyBindings.Clear();

        view.KeyBindings.AddRange(
            CompleteStatuses.Select(
                x => new KeyBinding
                {
                    [KeyBinding.CommandProperty] = CompleteCommand,
                    [KeyBinding.CommandParameterProperty] = x,
                    [KeyBinding.GestureProperty] = keys[x.Value],
                }
            )
        );
    }
}