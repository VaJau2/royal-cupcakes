using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Characters.Player;

public partial class MinimapItem : Node
{
	private Character player;
	private MiniMap miniMap;
	
	public override void _Ready()
	{
		miniMap = GetNode<MiniMap>("/root/Main/Level/Scene/canvas/miniMap");
		player = GetParent<Character>();
	}

	public void Initialize()
	{
		if (player.IsMultiplayerAuthority())
		{
			if (player.Team == Team.Thief)
			{
				miniMap.SetVisible(player);
			}
		}
		else
		{
			if (player.Team == Team.Guard)
			{
				miniMap.SpawnMarker(player);
			}
		}
	}
}
