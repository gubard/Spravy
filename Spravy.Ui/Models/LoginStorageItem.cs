namespace Spravy.Ui.Models;

[ProtoContract]
public class LoginStorageItem
{
    [ProtoMember(1)]
    public string? Token { get; set; }
}