using Spravy.Ui.Setting;

namespace Spravy.Ui.Features.ToDo.Services;

public abstract partial class IconViewModel : ViewModelBase, IStateHolder
{
    private readonly IObjectStorage objectStorage;

    [ObservableProperty]
    private string icon = string.Empty;

    private bool firstIconChanged = true;

    public abstract string ViewId { get; }

    public IconViewModel(IObjectStorage objectStorage)
    {
        this.objectStorage = objectStorage;
        PropertyChanged += OnPropertyChanged;
    }

    public AvaloniaList<string> FavoriteIcons { get; } = new();

    public Cvtar LoadStateAsync(CancellationToken ct)
    {
        return objectStorage
            .GetObjectOrDefaultAsync<AppSetting>(App.ViewId, ct)
            .IfSuccessAsync(
                setting =>
                    this.PostUiBackground(
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
            new AppSetting { FavoriteIcons = FavoriteIcons.ToArray(), },
            ct
        );
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
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
