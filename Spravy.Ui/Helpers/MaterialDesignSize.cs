using Avalonia.Layout;

namespace Spravy.Ui.Helpers;

public static class MaterialDesignSize
{
    public const double MinExtraSmall = 1;
    public const double MaxExtraSmall = 599;
    public const double MinSmall = 600;
    public const double MaxSmall = 1239;
    public const double MinMedium = 1240;
    public const double MaxMedium = 1439;
    public const double MinLarge = 1440;
    public const double MaxLarge = 1919;
    public const double MinExtraLarge = 1920;

    public static MaterialDesignSizeType LastType;

    public static event Action<MaterialDesignSizeType>? MaterialDesignSizeTypeChanged;

    static MaterialDesignSize()
    {
        Layoutable.WidthProperty.Changed.AddClassHandler<TopLevel>(
            (topLevel, _) =>
            {
                var currentType = topLevel.Width switch
                {
                    <= MaxExtraSmall => MaterialDesignSizeType.ExtraSmall,
                    > MaxExtraSmall and <= MaxSmall => MaterialDesignSizeType.Small,
                    > MaxSmall and <= MaxMedium => MaterialDesignSizeType.Medium,
                    > MaxMedium and <= MaxLarge => MaterialDesignSizeType.Large,
                    > MaxLarge => MaterialDesignSizeType.ExtraLarge,
                    _ => MaterialDesignSizeType.ExtraSmall
                };

                if (currentType == LastType)
                {
                    return;
                }

                LastType = currentType;
                RiseMaterialDesignSizeTypeChanged();
            }
        );

        if (LastType == MaterialDesignSizeType.ExtraSmall)
        {
            RiseMaterialDesignSizeTypeChanged();
        }
    }

    private static void RiseMaterialDesignSizeTypeChanged()
    {
        MaterialDesignSizeTypeChanged?.Invoke(LastType);
    }
}
