using Godot;
using RoyalCupcakes.Interface;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Characters.Player;

/**
 * Включает свободную камеру и выводит соответствующую надпись о поражении
 */
public partial class LosingHandler : Node
{
	[Export] private PackedScene freeCameraPrefab;
	
	private GameManager gameManager;
	private Character player;

	public override void _EnterTree()
	{
		player = GetParent<Character>();
		SetMultiplayerAuthority(player.PlayerId);
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		gameManager = GetNode<GameManager>("/root/Main/Level/Scene");
		CallDeferred(nameof(LoadDelayed));
	}

	public async void LoadDelayed()
	{
		await ToSignal(GetTree(), "process_frame");
		
		player.Tied += () =>
		{
			MainLabel.Instance.ShowText("#YOU_ARE_CAPTURED#");
			
			var scene = GetNode<Node3D>("/root/Main/Level/Scene");
			var freeCamera = freeCameraPrefab.Instantiate<FreeCamera>();
			scene.AddChild(freeCamera);
			freeCamera.GlobalPosition = player.GlobalPosition;
			freeCamera.Rotation = player.Rotation;
			freeCamera.MakeCurrent();
			
			var rotationController = GetNode<RotationController>("/root/Main/Level/Scene/rotationController");
			rotationController.AddRotatedObject(freeCamera);

			gameManager.RpcId(1,
				player.Team == Team.Thief
					? nameof(GameManager.RequestRemoveThief)
					: nameof(GameManager.RequestRemoveGuard));
		};
	}
}
