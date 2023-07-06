using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.ReactiveUI;
using ExtensionFramework.Core.DependencyInjection.Interfaces;
using ExtensionFramework.Core.DependencyInjection.Extensions;
using ExtensionFramework.Core.Ui.Models;
using Microsoft.Extensions.Configuration;
using Spravy.Core.Interfaces;
using Spravy.Models;
using Spravy.Profiles;
using Spravy.Services;
using Spravy.ViewModels;
using Spravy.Views;

namespace Spravy.Configurations;

public readonly struct SpravyDependencyInjectorConfiguration : IDependencyInjectorConfiguration
{
    public void Configure(IDependencyInjectorRegister register)
    {
        register.RegisterScope(() => new MapperConfiguration(cfg => cfg.AddProfile<SpravyProfile>()));
        register.RegisterScope<IMapper>((MapperConfiguration cfg) => new Mapper(cfg));
        register.RegisterScope<IToDoService, GrpcToDoService>();
        register.RegisterScope<Application, App>();
        register.RegisterScope(() => Enumerable.Empty<IDataTemplate>());
        register.RegisterScope<Control, MainView>();
        register.RegisterScope<Window, MainWindow>();
        register.RegisterScopeAutoInjectMember((MainWindow window) => window.Content, (Control control) => control);
        register.RegisterScope<RoutedViewHost>();

        register.RegisterScope<GrpcToDoServiceOptions>(
            (IConfiguration configuration) => new GrpcToDoServiceOptions
            {
                Host = configuration["GrpcToDoService:Host"],
            }
        );

        register.RegisterScope<IConfiguration>(
            () =>
                new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
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