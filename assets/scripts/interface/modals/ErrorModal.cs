using Godot;

namespace RoyalCupcakes.Interface.Modals;

public partial class ErrorModal : Control
{
	[Export] private AudioStreamPlayer audi;
	
	private void Cancel()
	{
		audi?.Play();
		Visible = false;
	}
}
