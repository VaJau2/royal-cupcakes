using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class ConnectingModal : Control
{
	[Export] 
	private AudioStreamPlayer audi;
	
	private void OnCancelConnecting()
	{
		audi?.Play();
		var main = GetNode<Main>("/root/Main");  
		main.Disconnect();
		Visible = false;
	}
}
