namespace Spravy.Ui.Features.ToDo.Settings;

public class AddToDoItemViewModelSettings : IViewModelSetting<AddToDoItemViewModelSettings>
{
    public static AddToDoItemViewModelSettings Default { get; }

    static AddToDoItemViewModelSettings()
    {
        Default = new();
    }

    public AddToDoItemViewModelSettings()
    {
        EditToDoItemViewModelSettings = EditToDoItemViewModelSettings.Default;
    }

    public AddToDoItemViewModelSettings(AddToDoItemViewModel viewModel)
    {
        EditToDoItemViewModelSettings = new(viewModel.EditToDoItemViewModel);
    }

    public EditToDoItemViewModelSettings EditToDoItemViewModelSettings { get; set; }
}
