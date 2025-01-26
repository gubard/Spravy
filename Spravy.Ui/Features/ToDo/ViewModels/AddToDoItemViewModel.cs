using Spravy.Picture.Domain.Interfaces;

namespace Spravy.Ui.Features.ToDo.ViewModels;

public class AddToDoItemViewModel : ToDoItemEditIdViewModel, IApplySettings
{
    private readonly IObjectStorage objectStorage;
    private readonly IToDoService toDoService;
    private readonly IPictureService pictureService;

    public AddToDoItemViewModel(
        Option<ToDoItemEntityNotify> editItem,
        ReadOnlyMemory<ToDoItemEntityNotify> editItems,
        IObjectStorage objectStorage,
        IToDoService toDoService,
        IPictureService pictureService,
        EditToDoItemViewModel editToDoItemViewModel
    ) : base(editItem, editItems)
    {
        this.objectStorage = objectStorage;
        this.toDoService = toDoService;
        EditToDoItemViewModel = editToDoItemViewModel;
        this.pictureService = pictureService;
    }

    public EditToDoItemViewModel EditToDoItemViewModel { get; }

    public override string ViewId =>
        EditItem.TryGetValue(out var editItem)
            ? $"{TypeCache<AddToDoItemViewModel>.Name}:{editItem.Id}"
            : TypeCache<AddToDoItemViewModel>.Name;

    public Cvtar ApplySettingsAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess
           .IfSuccessAsync(
                () =>
                {
                    if (ResultIds.IsEmpty)
                    {
                        return toDoService.AddToDoItemAsync(
                            new[]
                            {
                                EditToDoItemViewModel.GetAddToDoItemOptions(OptionStruct<Guid>.Default),
                            },
                            ct
                        );
                    }

                    return ResultIds.ToResult()
                       .IfSuccessForEach(x => EditToDoItemViewModel.GetAddToDoItemOptions(x.ToOption()).ToResult())
                       .IfSuccessAsync(options => toDoService.AddToDoItemAsync(options, ct), ct);
                },
                ct
            )
           .IfSuccessAsync(
                toDoIds => pictureService.EditPictureAsync(EditToDoItemViewModel.GetEditPicture(toDoIds), ct),
                ct
            );
    }

    public Result UpdateItemUi()
    {
        return Result.Success;
    }

    public override Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<AddToDoItemViewModelSettings>(ViewId, ct)
           .IfSuccessAsync(
                setting => this.PostUiBackground(
                    () =>
                    {
                        EditToDoItemViewModel.SetItem(setting.EditToDoItemViewModelSettings);

                        return Result.Success;
                    },
                    ct
                ),
                ct
            )
           .IfSuccessAsync(() => EditToDoItemViewModel.LoadStateAsync(ct), ct);
    }

    public override Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(ViewId, new AddToDoItemViewModelSettings(this), ct)
           .IfSuccessAsync(() => EditToDoItemViewModel.SaveStateAsync(ct), ct);
    }

    public override Cvtar RefreshAsync(CancellationToken ct)
    {
        return Result.AwaitableSuccess;
    }
}