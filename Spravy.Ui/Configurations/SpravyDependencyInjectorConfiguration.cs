using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.Common.Interfaces;
using ExtensionFramework.Core.Common.Services;
using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.Ui.Models;
using ExtensionFramework.ReactiveUI.Interfaces;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Configuration;
using Spravy.Core.Interfaces;
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
        register.RegisterScope(() => new MapperConfiguration(cfg => cfg.AddProfile<SpravyProfile>()));
        register.RegisterScope<IMapper>((MapperConfiguration cfg) => new Mapper(cfg));
        register.RegisterScope<IToDoService, GrpcToDoService>();
        register.RegisterScope<IExceptionViewModel, ExceptionViewModel>();
        register.RegisterScope(() => Enumerable.Empty<IDataTemplate>());
        register.RegisterScope<Control, MainView>();
        register.RegisterScope<Window, MainWindow>();
        register.RegisterScopeAutoInjectMember((MainWindow window) => window.Content, (Control control) => control);
        register.RegisterScope<RoutedViewHost>();
        register.RegisterScope<Application, App>();
        register.RegisterScope<IResourceLoader, FileResourceLoader>();

        register.RegisterScopeDel<GrpcToDoServiceOptions>(
            (IConfiguration configuration) =>
            {
#if DEBUG
                return new GrpcToDoServiceOptions
                {
                    Mode = GrpcWebMode.GrpcWeb,
                    Host = "http://192.168.50.2:5000",
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
                        typeof(AddToDoItemViewModel), typeof(ToDoItemViewModel)
                    }
                }
            )
        );
    }
}