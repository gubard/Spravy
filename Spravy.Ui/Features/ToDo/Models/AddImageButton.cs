namespace Spravy.Ui.Features.ToDo.Models;

public class AddImageButton
{
    public AddImageButton(SpravyCommand addImageCommand)
    {
        AddImageCommand = addImageCommand;
    }

    public SpravyCommand AddImageCommand { get; }
}