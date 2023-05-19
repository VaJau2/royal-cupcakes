using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class ConnectingModal : Window
{
	private void OnCancelConnecting()
	{
		var main = GetNode<Main>("/root/Main");  
		main.Disconnect();
		Hide();
	}
}
