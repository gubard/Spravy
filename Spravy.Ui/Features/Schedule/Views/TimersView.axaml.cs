using Spravy.Ui.Features.Schedule.ViewModels;

namespace Spravy.Ui.Features.Schedule.Views;

public partial class TimersView : UserControl
{
    public TimersView()
    {
        InitializeComponent();

        Initialized += (_, _) =>
        {
            if (DataContext is not TimersViewModel vm)
            {
                return;
            }

            vm.InitializedCommand.Command.Execute(null);
        };
    }
}
