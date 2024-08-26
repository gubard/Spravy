namespace Spravy.Ui.Features.ToDo.Settings;

public class ResetToDoItemViewModelSetting : IViewModelSetting<ResetToDoItemViewModelSetting>
{
    public ResetToDoItemViewModelSetting(ResetToDoItemViewModel viewModel)
    {
        IsCompleteChildrenTask = viewModel.IsCompleteChildrenTask;
        IsMoveCircleOrderIndex = viewModel.IsMoveCircleOrderIndex;
        IsOnlyCompletedTasks = viewModel.IsOnlyCompletedTasks;
        IsCompleteCurrentTask = viewModel.IsCompleteCurrentTask;
    }

    public ResetToDoItemViewModelSetting() { }

    static ResetToDoItemViewModelSetting()
    {
        Default = new() { IsMoveCircleOrderIndex = true };
    }

    public bool IsCompleteChildrenTask { get; set; }
    public bool IsMoveCircleOrderIndex { get; set; }
    public bool IsOnlyCompletedTasks { get; set; }
    public bool IsCompleteCurrentTask { get; set; }
    public static ResetToDoItemViewModelSetting Default { get; }
}
