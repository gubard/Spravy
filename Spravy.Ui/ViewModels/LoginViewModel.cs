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

public class LoginViewModel : RoutableViewModelBase
{
    private string login = string.Empty;
    private string password = string.Empty;

    public LoginViewModel() : base("login")
    {
        LoginCommand = CreateCommandFromTaskWithDialogProgressIndicator(LoginAsync);
        CreateUserCommand = CreateCommand(CreateUser);
    }

    [Inject]
    public required IAuthenticationService AuthenticationService { get; init; }

    [Inject]
    public required IMapper Mapper { get; init; }

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

    public ICommand LoginCommand { get; }
    public ICommand CreateUserCommand { get; }

    private void CreateUser()
    {
        Navigator.NavigateTo<CreateUserViewModel>();
    }

    private async Task LoginAsync()
    {
        var user = Mapper.Map<User>(this);
        var isValid = await AuthenticationService.IsValidAsync(user);

        if (!isValid)
        {
            throw new Exception("Login error.");
        }

        Navigator.NavigateTo<RootToDoItemViewModel>();
    }
}