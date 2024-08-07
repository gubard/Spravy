namespace Spravy.Ui.Views;

public partial class SettingView : UserControl
{
    public SettingView()
    {
        InitializeComponent();

        this.AddAdaptiveStyle(
            new[]
            {
                MaterialDesignSizeType.Medium,
                MaterialDesignSizeType.Large,
                MaterialDesignSizeType.ExtraLarge
            },
            "BorderAdaptive"
        );

        this.AddAdaptiveStyle(
            new[]
            {
                MaterialDesignSizeType.Medium,
                MaterialDesignSizeType.Large,
                MaterialDesignSizeType.ExtraLarge
            },
            "ItemsControlAdaptive"
        );
    }
}
