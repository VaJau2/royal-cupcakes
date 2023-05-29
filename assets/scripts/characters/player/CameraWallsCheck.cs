using Godot;

namespace RoyalCupcakes.Characters.Player;

public partial class CameraWallsCheck : RayCast3D
{
	private const float PositionSpeed = 8f;
	private const float RotationSpeed = 4f;
	private const float CloseDistance = 0.1f;
	
	[Export] private Camera3D camera;
	[Export] private Node3D closePositionNode;
	private Vector3 startPos, startRot;

	private bool seePlayer = true;
	private bool cameraInPlace = true;

	public override void _Ready()
	{
		if (!GetParent().IsMultiplayerAuthority())
		{
			SetProcess(false);
			return;
		}
		
		startPos = camera.Position;
		startRot = camera.Rotation;
	}

	public override void _Process(double delta)
	{
		var tempSeePlayer = GetSeePlayer();
		if (seePlayer != tempSeePlayer)
		{
			seePlayer = tempSeePlayer;
			cameraInPlace = false;
		}

		if (cameraInPlace) return;

		var targetPos = seePlayer
			? startPos
			: closePositionNode.Position;

		var targetRot = seePlayer
			? startRot
			: closePositionNode.Rotation;
		
		camera.Position = camera.Position.MoveToward(targetPos, PositionSpeed * (float)delta);
		camera.Rotation = camera.Rotation.MoveToward(targetRot, RotationSpeed * (float)delta);
		cameraInPlace = camera.Position.DistanceTo(targetPos) < CloseDistance;
	}

	private bool GetSeePlayer() => GetCollider() is Character;
}
