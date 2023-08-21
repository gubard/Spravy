using System;
using System.Linq;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.ReactiveUI;
using Material.Styles.Controls;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Spravy.Authentication.Domain.Core.Profiles;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;
using Spravy.Domain.Interfaces;
using Spravy.Domain.Extensions;
using Spravy.Domain.Models;
using Spravy.Domain.Services;
using Spravy.ToDo.Domain.Core.Profiles;
using Spravy.ToDo.Domain.Interfaces;
using Spravy.Ui.Controls;
using Spravy.Ui.Enums;
using Spravy.Ui.Extensions;
using Spravy.Ui.Interfaces;
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
        register.RegisterScopeAutoInjectMember((RoutedViewHost host) => host.Router, (RoutingState state) => state);
        register.RegisterScope<IModuleSetup, ReactiveUIModuleSetup>();
        register.RegisterScope<IViewLocator, ModuleViewLocator>();
        register.RegisterScope<INavigator, Navigator>();
        register.RegisterSingleton(new RoutingState());
        register.RegisterTransient<PathControl>();
        register.RegisterScope(() => new MapperConfiguration(SetupMapperConfiguration));
        register.RegisterScope<IMapper>((MapperConfiguration cfg) => new Mapper(cfg));
        register.RegisterScope<IToDoService, GrpcToDoService>();
        register.RegisterScope<IExceptionViewModel, ExceptionViewModel>();
        register.RegisterScope(() => Enumerable.Empty<IDataTemplate>());
        register.RegisterScope<Control, MainView>();
        register.RegisterSingleton<Window>((MainWindow window) => window);
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
        register.RegisterTransient<IAuthenticationService, GrpcAuthenticationService>();
        register.RegisterTransient<IKeeper<TokenResult>, StaticKeeper<TokenResult>>();
        RegisterViewModels(register);

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

        register.RegisterScopeDel<GrpcAuthenticationServiceOptions>(
            (IConfiguration configuration) =>
            {
#if DEBUG
                return new GrpcAuthenticationServiceOptions
                {
                    ChannelType = GrpcChannelType.Default,
                    Host = "http://localhost:5001",
                    ChannelCredentialType = ChannelCredentialType.Insecure
                };
#endif
                var options = configuration.GetSection("GrpcAuthenticationService")
                    .Get<GrpcAuthenticationServiceOptions>();

                return options;
            }
        );

        register.RegisterScope(() => new AppConfiguration(typeof(LoginViewModel)));
    }

    private static void SetupMapperConfiguration(IMapperConfigurationExpression expression)
    {
        expression.AddProfile<SpravyToDoProfile>();
        expression.AddProfile<SpravyUiProfile>();
        expression.AddProfile<SpravyAuthenticationProfile>();
    }

    private void RegisterViewModels(IDependencyInjectorRegister register)
    {
        var styledElementType = typeof(StyledElement);
        var member = styledElementType.GetProperty(nameof(StyledElement.DataContext)).ThrowIfNull();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var autoInjectMember = new AutoInjectMember(member);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.Namespace.IsNullOrWhiteSpace())
                {
                    continue;
                }

                if (!styledElementType.IsAssignableFrom(type))
                {
                    continue;
                }

                var ns = type.Namespace
                    .Replace(".Views.", ".ViewModels.")
                    .Replace(".Views", ".ViewModels");

                var viewModelName = $"{ns}.{type.Name}Model";
                var viewModelType = assembly.GetType(viewModelName);

                if (viewModelType is null)
                {
                    continue;
                }

                var autoInjectIdentifier = new AutoInjectMemberIdentifier(type, autoInjectMember);
                var variable = viewModelType.ToVariableAutoName();
                register.RegisterScope(type);
                register.RegisterScope(viewModelType);

                register.RegisterScopeAutoInjectMember(
                    autoInjectIdentifier,
                    variable.ToLambda(variable)
                );
            }
        }
    }
}