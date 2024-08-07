namespace Spravy.Ui.Interfaces;

public interface IViewSelector
{
    Result<Control> GetView(Type viewModelType);
}
