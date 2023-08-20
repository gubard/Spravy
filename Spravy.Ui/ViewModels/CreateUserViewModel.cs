using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using ExtensionFramework.Core.DependencyInjection.Attributes;
using ExtensionFramework.ReactiveUI.Models;
using ReactiveUI;
using Spravy.Authentication.Domain.Interfaces;
using Spravy.Authentication.Domain.Models;

namespace Spravy.Ui.ViewModels;

public class CreateUserViewModel : RoutableViewModelBase
{
    private string login = string.Empty;
    private string password = string.Empty;
    private string repeatPassword = string.Empty;

    public CreateUserViewModel() : base("create-user")
    {
        CreateUserCommand = CreateCommandFromTaskWithDialogProgressIndicator(CreateUserAsync);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

    public ICommand CreateUserCommand { get; }

    public string Login
    {
        get => login;
        set => this.RaiseAndSetIfChanged(ref login, value);
    }

    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    public string RepeatPassword
    {
        get => repeatPassword;
        set => this.RaiseAndSetIfChanged(ref repeatPassword, value);
    }

    private async Task CreateUserAsync()
    {
        if (Password != RepeatPassword)
        {
            throw new Exception("Password not equal RepeatPassword.");
        }

        var options = Mapper.Map<CreateUserOptions>(this);
        await AuthenticationService.CreateUserAsync(options);
        Navigator.NavigateTo<LoginViewModel>();
    }
}