using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface.Modals;

public partial class DebugModal : AbstractModal
{
	private Main main;
	
	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
	}

	private void StartDebug(bool isThief)
	{
		main.PlayerTeam = isThief ? Team.Thief : Team.Guard;
		main.StartDebugHost();
	}
}
