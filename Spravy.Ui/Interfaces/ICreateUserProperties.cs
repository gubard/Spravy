namespace Spravy.Ui.Interfaces;

public interface ICreateUserProperties
{
    string Email { get; set; }
    string Login { get; set; }
    string Password { get; set; }
    string RepeatPassword { get; set; }
}
