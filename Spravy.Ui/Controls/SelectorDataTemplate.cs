namespace Spravy.Ui.Controls;

public class SelectorDataTemplate : AvaloniaList<IDataTemplate>, IDataTemplate
{
    public SelectorDataTemplate()
    {
        ResetBehavior = ResetBehavior.Remove;
    }

    public Control? Build(object? param)
    {
        return this.FirstOrDefault(x => x.Match(param))?.Build(param);
    }

    public bool Match(object? data)
    {
        return this.Any(x => x.Match(data));
    }
}