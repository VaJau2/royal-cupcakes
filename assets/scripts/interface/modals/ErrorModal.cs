using Godot;

namespace RoyalCupcakes.Interface.Modals;

public partial class ErrorModal : AbstractModal
{
    private const string DefaultError = "#CONNECTION_ERROR#";
    
    private Label errorLabel;

    public override void _Ready()
    {
        errorLabel = GetNode<Label>("Label");
    }

    public string ErrorText
    {
        get => errorLabel.Text;
        set => errorLabel.Text = value;
    }

    protected override void CloseModal()
    {
        ErrorText = DefaultError;
        base.CloseModal();
    }
}
