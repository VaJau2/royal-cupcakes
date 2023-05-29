using Godot;

namespace RoyalCupcakes.Characters.Player;

/**
 * Камера включается, если текущий игрок проигрывает
 * позволяет летать по карте и осматриваться
 */
public partial class FreeCamera : CharacterBody3D
{
	private const float Speed = 8f;
	private const float Acceleration = 0.4f;

	private Camera3D camera;

	public void MakeCurrent()
	{
		camera.Current = true;
	}

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("camera");
	}

	public override void _Process(double delta)
	{
		var inputDir = Input.GetVector(
			"ui_left", "ui_right",
			"ui_up", "ui_down"
		);
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		Velocity = Velocity.MoveToward(direction * Speed, Acceleration);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!(Velocity.Length() > 0)) return;
		MoveAndSlide();
	}
}
