using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Input;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
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
        CompleteCommand = CreateCommandFromTaskWithDialogProgressIndicator<CompleteStatus>(CompleteAsync);
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

    public void SetCompleteStatus()
    {
        CompleteStatuses.Clear();

        CompleteStatuses.AddRange(
            new Ref<CompleteStatus>[]
            {
                new(CompleteStatus.Complete),
                new(CompleteStatus.Skip),
                new(CompleteStatus.Fail),
            }
        );

        UpdateKeyBindings();
    }

    public void SetIncompleteStatus()
    {
        CompleteStatuses.Clear();

        CompleteStatuses.AddRange(
            new Ref<CompleteStatus>[]
            {
                new(CompleteStatus.Incomplete),
                new(CompleteStatus.Skip),
                new(CompleteStatus.Fail),
            }
        );

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