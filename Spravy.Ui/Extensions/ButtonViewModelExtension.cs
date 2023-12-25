using Ninject;
using Spravy.Domain.Di.Helpers;
using Spravy.Domain.Extensions;
using Spravy.Ui.ViewModels;

namespace Spravy.Ui.Extensions;

public static class ButtonViewModelExtension
{
    public static ButtonViewModel WithParameter(this ButtonViewModel viewModel, object parameter)
    {
        var kernel = DiHelper.Kernel.ThrowIfNull();
        var result = kernel.Get<ButtonViewModel>();
        result.Command = viewModel.Command;
        result.Name = viewModel.Name;
        result.Icon = viewModel.Icon;
        result.Work = viewModel.Work;
        result.Command = viewModel.Command;
        result.Parameter = parameter;

        return result;
    }
}