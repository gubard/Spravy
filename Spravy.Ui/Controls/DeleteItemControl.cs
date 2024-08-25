namespace Spravy.Ui.Controls;

[TemplatePart(Name = "PART_TimePicker", Type = typeof(Button))]
public partial class DeleteItemControl : ContentControl
{
    public EventHandler? ClickDelete;

    private Button? deleteButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        deleteButton = e.NameScope.Find<Button>("PART_DeleteButton");

        if (deleteButton is not null)
        {
            deleteButton.Click += (_, _) => ClickDelete?.Invoke(this, EventArgs.Empty);
        }

        base.OnApplyTemplate(e);
    }
}
