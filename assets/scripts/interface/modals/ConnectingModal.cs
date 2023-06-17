using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class ConnectingModal : AbstractModal
{
	private void OnCancelConnecting()
	{
		audi?.Play();
		var main = GetNode<Main>("/root/Main");  
		main.Disconnect();
		Visible = false;
	}
}
