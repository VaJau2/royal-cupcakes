using Godot;

namespace RoyalCupcakes.Interface.Modals;

public abstract partial class AbstractModal: Control
{
    [Export] protected AudioStreamPlayer audi;
    
    public void OpenModal()
    {
        Visible = true;
    }
    
    protected virtual void CloseModal()
    {
        audi?.Play();
        Visible = false;
    }
}
