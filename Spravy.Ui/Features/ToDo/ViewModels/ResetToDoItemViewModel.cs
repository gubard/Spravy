using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Ninject;
using ProtoBuf;
using ReactiveUI.Fody.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Domain.Helpers;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Models;
using Spravy.Ui.Extensions;
using Spravy.Ui.Models;
using Spravy.Ui.Services;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class ResetToDoItemViewModel : NavigatableViewModelBase
{
    public ResetToDoItemViewModel() : base(true)
    {
        InitializedCommand = CreateInitializedCommand(TaskWork.Create(InitializedAsync).RunAsync);
    }

    public ICommand InitializedCommand { get; }
    [Reactive] public Guid Id { get; set; }
    [Reactive] public bool IsCompleteTask { get; set; }
    [Reactive] public bool IsMoveCircleOrderIndex { get; set; } = true;
    [Inject] public required IObjectStorage ObjectStorage { get; init; }
    public override string ViewId => $"{TypeCache<ResetToDoItemViewModel>.Type.Name}:{Id}";

    private ConfiguredValueTaskAwaitable<Result> InitializedAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.GetObjectOrDefaultAsync<ResetToDoItemViewModelSetting>(ViewId, cancellationToken)
            .IfSuccessAsync(obj => SetStateAsync(obj, cancellationToken), cancellationToken);
    }

    public override Result Stop()
    {
        return Result.Success;
    }

    public override ConfiguredValueTaskAwaitable<Result> SetStateAsync(
        object setting,
        CancellationToken cancellationToken
    )
    {
        return setting.CastObject<ResetToDoItemViewModelSetting>()
            .IfSuccessAsync(
                s => this.InvokeUIBackgroundAsync(
                    () =>
                    {
                        IsCompleteTask = s.IsCompleteTask;
                        IsMoveCircleOrderIndex = s.IsMoveCircleOrderIndex;
                    }
                ),
                cancellationToken
            );
    }

    public override ConfiguredValueTaskAwaitable<Result> SaveStateAsync(CancellationToken cancellationToken)
    {
        return ObjectStorage.SaveObjectAsync(ViewId, new ResetToDoItemViewModelSetting(this));
    }

    [ProtoContract]
    private class ResetToDoItemViewModelSetting
    {
        public ResetToDoItemViewModelSetting(ResetToDoItemViewModel viewModel)
        {
            IsCompleteTask = viewModel.IsCompleteTask;
            IsMoveCircleOrderIndex = viewModel.IsMoveCircleOrderIndex;
        }

        public ResetToDoItemViewModelSetting()
        {
        }


        [ProtoMember(1)] public bool IsCompleteTask { get; set; }
        [ProtoMember(2)] public bool IsMoveCircleOrderIndex { get; set; }
    }
}