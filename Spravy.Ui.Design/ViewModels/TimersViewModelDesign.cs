using Spravy.Schedule.Domain.Models;
using Spravy.Ui.Design.Helpers;
using Spravy.Ui.Design.Services;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Design.ViewModels;

public class TimersViewModelDesign : TimersViewModel
{
    public TimersViewModelDesign()
    {
        Mapper = ConstDesign.Mapper;

        ScheduleService = new ScheduleServiceDesign(
            new TimerItem[]
            {
                new(DateTimeOffset.Now, Guid.NewGuid(), Array.Empty<byte>(), Guid.NewGuid())
            }
        );
    }
}