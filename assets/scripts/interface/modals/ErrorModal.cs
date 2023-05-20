using Godot;

namespace RoyalCupcakes.Interface.Modals;

public partial class ErrorModal : Control
{
	private void Cancel()
	{
		Visible = false;
	}
}
