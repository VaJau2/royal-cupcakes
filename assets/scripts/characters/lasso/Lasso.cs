using Godot;

namespace RoyalCupcakes.Characters.Lasso;

/**
 * Двигает лассо и ловит подозрительных поней
 */
public partial class Lasso : RigidBody3D
{
	public LassoHandler LassoHandler { get; set; }
	
	private RopeHandler myRope;
	private double timer = 2f;
	
	public override void _EnterTree()
	{
		var id = int.Parse(Name.ToString().Split('_')[1]);
		SetMultiplayerAuthority(id);
	}

	public void MoveToTarget(Vector3 targetPosition)
	{
		var dir = GlobalPosition.DirectionTo(targetPosition);
		var force = GlobalPosition.DistanceTo(targetPosition);
		
		dir.Y += 0.1f;
		force = Mathf.Clamp(force, 1, 3);
		
		LinearVelocity = dir * force * 3f;
	}

	public override void _Ready()
	{
		myRope = GetNode<RopeHandler>("rope");
	}

	public override void _Process(double delta)
	{
		myRope.SetPoints(GlobalPosition, LassoHandler.LassoPosition);
		if (!IsMultiplayerAuthority()) return;

		if (timer > 0 && LinearVelocity.Length() > 0.05f)
		{
			timer -= delta;
			return;
		}

		Rpc(nameof(Disable));
	}
	
	private void OnCatchBody(Node3D body)
	{
		if (body is not Character character) return;
		if (character.IsTied) return;
		if (character == LassoHandler.PlayerOwner) return;

		if (!IsMultiplayerAuthority()) return;
		LassoHandler.HandleCaughtCharacter(character);
		Rpc(nameof(Disable));
		character.Rpc(nameof(Character.SetIsTied), true);
	}

	[Rpc(CallLocal = true)]
	private void Disable()
	{
		QueueFree();
		LassoHandler.ResetLasso();
	}
}
