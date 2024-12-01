namespace Spravy.Ui.Helpers;

public static class Regexs
{
    public static readonly Regex LineIsEmail = new(
        @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"
    );

    public static readonly Regex LineIsPassword = new("^[ !@#$%^&*()_+\\-={}\\[\\]:;\"'<>,.?/\\\\|a-zA-Z0-9]+$");

    public static readonly Regex LineIsLogin = new("^[!@#$%^&*()_+\\-={}\\[\\]:;\"'<>,.?/\\\\|a-zA-Z0-9]+$");
}