using Godot;
using RoyalCupcakes.Interface;

namespace RoyalCupcakes.Characters.Player;

/**
 * Включает свободную камеру и выводит соответствующую надпись о поражении
 */
public partial class LosingHandler : Node
{
	[Export] private PackedScene freeCameraPrefab;
	
	private Character player;

	public override void _EnterTree()
	{
		player = GetParent<Character>();
		SetMultiplayerAuthority(player.PlayerId);
	}

	public override void _Ready()
	{
		if (!IsMultiplayerAuthority()) return;
		CallDeferred(nameof(LoadDelayed));
	}

	public async void LoadDelayed()
	{
		await ToSignal(GetTree(), "process_frame");
		
		player.Tied += () =>
		{
			MainLabel.Instance.ShowText("#YOU_ARE_CAPTURED#");
			
			var scene = GetNode<Node3D>("/root/Main/Level/Scene");
			var playerOldCamera = player.GetNode<Node3D>("camera");
			var freeCamera = freeCameraPrefab.Instantiate<FreeCamera>();
			scene.AddChild(freeCamera);
			freeCamera.SetPosition(player.GlobalPosition, playerOldCamera.Position);
			freeCamera.MakeCurrent();
		};
	}
}
