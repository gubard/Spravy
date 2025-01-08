using Spravy.Ui.Setting;

namespace Spravy.Ui.Features.ToDo.Services;

public abstract partial class IconViewModel : ViewModelBase, IStateHolder
{
    private readonly IObjectStorage objectStorage;

    private bool firstIconChanged = true;

    [ObservableProperty]
    private string icon = string.Empty;

    public IconViewModel(IObjectStorage objectStorage)
    {
        this.objectStorage = objectStorage;
    }

    public AvaloniaList<string> FavoriteIcons { get; } = new();

    public abstract string ViewId { get; }

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage.GetObjectOrDefaultAsync<AppSetting>(App.ViewId, ct)
           .IfSuccessAsync(
                setting => this.PostUiBackground(
                    () =>
                    {
                        FavoriteIcons.UpdateUi(setting.FavoriteIcons);

                        return Result.Success;
                    },
                    ct
                ),
                ct
            );
    }

    public Cvtar SaveStateAsync(CancellationToken ct)
    {
        return objectStorage.SaveObjectAsync(
            App.ViewId,
            new AppSetting
            {
                FavoriteIcons = FavoriteIcons.ToArray(),
            },
            ct
        );
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Icon))
        {
            if (firstIconChanged)
            {
                firstIconChanged = false;

                return;
            }

            if (Icon.IsNullOrWhiteSpace())
            {
                return;
            }

            if (FavoriteIcons.Contains(Icon))
            {
                return;
            }

            FavoriteIcons.Insert(0, Icon);

            if (FavoriteIcons.Count > 5)
            {
                FavoriteIcons.RemoveAt(5);
            }
        }
    }
}