namespace Spravy.Ui.Services;

public class PropertyValidator : IPropertyValidator
{
    public string? ValidEmail(string? email, string propertyName)
    {
        if (email.IsNullOrWhiteSpace())
        {
            return $"{propertyName} can't be empty.";
        }

        if (Regexs.LineIsEmail.IsMatch(email))
        {
            return null;
        }

        return $"{propertyName} is not valid email.";
    }

    public string? ValidPassword(string? password, string propertyName)
    {
        if (password.IsNullOrWhiteSpace())
        {
            return $"{propertyName} can't be empty.";
        }

        if (Regexs.LineIsPassword.IsMatch(password))
        {
            return null;
        }

        return $"{propertyName} is not valid password.";
    }

    public string? ValidLogin(string? login, string propertyName)
    {
        if (login.IsNullOrWhiteSpace())
        {
            return $"{propertyName} can't be empty.";
        }

        if (Regexs.LineIsLogin.IsMatch(login))
        {
            return null;
        }

        return $"{propertyName} is not valid login.";
    }

    public string? ValidLength(string? text, ushort min, ushort max, string propertyName)
    {
        if (text is null)
        {
            return $"{propertyName} can't be null.";
        }

        if (text.Length < min)
        {
            return $"{propertyName} can't be less when {min}.";
        }

        if (text.Length > max)
        {
            return $"{propertyName} can't be more when {max}.";
        }

        return null;
    }

    public string? ValidEquals(string? x, string? y, string propertyNameX, string propertyNameY)
    {
        if (x.IsNullOrWhiteSpace())
        {
            return $"{propertyNameX} can't be empty.";
        }

        if (y.IsNullOrWhiteSpace())
        {
            return $"{propertyNameY} can't be empty.";
        }

        if (x != y)
        {
            return $"{propertyNameX} not equal {propertyNameY}.";
        }

        return null;
    }
}
