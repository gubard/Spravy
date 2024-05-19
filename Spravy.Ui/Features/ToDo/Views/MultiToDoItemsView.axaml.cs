using Avalonia.Data;

namespace Spravy.Ui.Features.ToDo.Views;

[PseudoClasses(PseudoClassesIsMulti)]
public partial class MultiToDoItemsView : ReactiveUserControl<MultiToDoItemsViewModel>
{
    public const string ContentContentControlName = "content-content-control";
    public const string PseudoClassesIsMulti = ":is-multi";
    
    public static readonly StyledProperty<bool> IsMultiProperty =
        AvaloniaProperty.Register<MultiToDoItemsView, bool>(nameof(IsMulti), defaultBindingMode: BindingMode.TwoWay);
    
    public MultiToDoItemsView()
    {
        InitializeComponent();
    }
    
    public bool IsMulti
    {
        get => GetValue(IsMultiProperty);
        set
        {
            SetValue(IsMultiProperty, value);
            UpdatePseudoClasses();
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        UpdatePseudoClasses();
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(PseudoClassesIsMulti, IsMulti);
    }
}