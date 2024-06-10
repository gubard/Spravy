namespace Spravy.Ui.Errors;

public class NavigatorCacheEmptyError : Error
{
    public static readonly Guid MainId = new("E0ED2CBA-0E0F-4132-A983-764AB79717D6");
    
    public NavigatorCacheEmptyError() : base(MainId)
    {
    }
    
    public override string Message
    {
        get => "Navigator cache empty.";
    }
}