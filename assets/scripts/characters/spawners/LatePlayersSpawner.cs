using Godot;
using RoyalCupcakes.Characters.Player;
using RoyalCupcakes.System;

/**
 * Спавнит летающую камеру вместо игрока, если тот подключается к уже идущей игре
 */
public partial class LatePlayersSpawner : Node3D
{
	[Export] private PackedScene freeCameraPrefab;
	private Main main;
	
	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
		if (main.PlayerTeam != Team.Npc) return;
	
		CallDeferred(nameof(SpawnCamera));
	}

	private void SpawnCamera()
	{
		var scene = GetNode<Node3D>("/root/Main/Level/Scene");
		var freeCamera = freeCameraPrefab.Instantiate<FreeCamera>();
		scene.AddChild(freeCamera);
		freeCamera.GlobalPosition = GlobalPosition;
		freeCamera.MakeCurrent();
		
		CallDeferred(nameof(DisableBlackScreenDeferred));
	}
	
	private async void DisableBlackScreenDeferred()
	{
		await ToSignal(GetTree(), "process_frame");
		var blackScreen = GetNode<ColorRect>("/root/Main/Level/Scene/canvas/blackScreen");
		blackScreen.Visible = false;
	}
}
