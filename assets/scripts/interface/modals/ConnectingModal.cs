using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class ConnectingModal : Control
{
	private void OnCancelConnecting()
	{
		var main = GetNode<Main>("/root/Main");  
		main.Disconnect();
		Visible = false;
	}
}
