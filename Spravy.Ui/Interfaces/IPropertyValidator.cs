namespace Spravy.Ui.Interfaces;

public interface IPropertyValidator
{
    string? ValidEmail(string? email, string propertyName);
    string? ValidPassword(string? password, string propertyName);
    string? ValidLogin(string? login, string propertyName);
    string? ValidLength(string? text, ushort min, ushort max, string propertyName);
    string? ValidEquals(string? x, string? y, string propertyNameX, string propertyNameY);
}
