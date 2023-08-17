using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using ExtensionFramework.AvaloniaUi.Controls;
using ExtensionFramework.AvaloniaUi.Extensions;
using ExtensionFramework.Core.Common.Extensions;
using ExtensionFramework.Core.Common.Interfaces;
using ExtensionFramework.Core.Common.Services;
using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.Ui.Interfaces;
using ExtensionFramework.Core.Ui.Models;
using ExtensionFramework.ReactiveUI.Interfaces;
using Material.Styles.Controls;
using Microsoft.Extensions.Configuration;
using Spravy.ToDo.Domain.Core.Profiles;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Enums;
using Spravy.Ui.Models;
using Spravy.Ui.Profiles;
using Spravy.Ui.Services;
using Spravy.Ui.ViewModels;
using Spravy.Ui.Views;

namespace Spravy.Ui.Configurations;

public readonly struct SpravyDependencyInjectorConfiguration : IDependencyInjectorConfiguration
{
    public void Configure(IDependencyInjectorRegister register)
    {
        register.RegisterScope(() => new MapperConfiguration(SetupMapperConfiguration));
        register.RegisterScope<IMapper>((MapperConfiguration cfg) => new Mapper(cfg));
        register.RegisterScope<IToDoService, GrpcToDoService>();
        register.RegisterScope<IExceptionViewModel, ExceptionViewModel>();
        register.RegisterScope(() => Enumerable.Empty<IDataTemplate>());
        register.RegisterScope<Control, MainView>();
        register.RegisterSingleton<Window, MainWindow>();
        register.RegisterScopeAutoInjectMember((MainWindow window) => window.Content, (Control control) => control);
        register.RegisterScope<RoutedViewHost>();
        register.RegisterScope<Application, App>();
        register.RegisterScope<IResourceLoader, FileResourceLoader>();
        register.RegisterTransient<AnnuallyPeriodicityViewModel>();
        register.RegisterTransient<DailyPeriodicityViewModel>();
        register.RegisterTransient<MonthlyPeriodicityViewModel>();
        register.RegisterTransient<WeeklyPeriodicityViewModel>();
        register.RegisterTransient<DayOfYearSelector>();
        register.RegisterTransient<DayOfWeekSelector>();
        register.RegisterTransient<DayOfMonthSelector>();

        register.RegisterTransient(
            () => Application.Current.ThrowIfNull("Application").GetTopLevel().ThrowIfNull("TopLevel").Clipboard
        );

        register.RegisterScope<IDialogViewer>(
            (IResolver resolver) => new DialogViewer(DialogViewer.DefaultDialogIdentifier)
            {
                Resolver = resolver
            }
        );

        register.RegisterTransient<IDialogProgressIndicator>(
            () => new Card
            {
                CornerRadius = new CornerRadius(24),
                Padding = new Thickness(4),
                Margin = new Thickness(4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Content = new ProgressBar
                {
                    Classes =
                    {
                        "circular"
                    },
                    Height = 100,
                    Width = 100,
                    IsIndeterminate = true,
                }
            }
        );

        register.RegisterScopeDel<GrpcToDoServiceOptions>(
            (IConfiguration configuration) =>
            {
#if DEBUG
                return new GrpcToDoServiceOptions
                {
                    ChannelType = GrpcChannelType.Default,
                    Host = "http://localhost:5000",
                    ChannelCredentialType = ChannelCredentialType.Insecure
                };
#endif
                var options = configuration.GetSection("GrpcToDoService").Get<GrpcToDoServiceOptions>();

                return options;
            }
        );

        register.RegisterScope(
            () => new AppConfiguration(
                typeof(RootToDoItemViewModel),
                new Dictionary<Type, Type>
                {
                    {
                        typeof(AddRootToDoItemViewModel), typeof(RootToDoItemViewModel)
                    },
                    {
                        typeof(AddToDoItemViewModel), typeof(ToDoItemValueViewModel)
                    }
                }
            )
        );
    }

    private static void SetupMapperConfiguration(IMapperConfigurationExpression expression)
    {
        expression.AddProfile<SpravyProfile>();
        expression.AddProfile<SpravyUiProfile>();
    }
}