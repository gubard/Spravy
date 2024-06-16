namespace Spravy.Ui.Interfaces;

public interface IViewSelector
{
    Result<IViewFor> GetView(Type viewModelType);
}