using Godot;

namespace RoyalCupcakes.Characters;

/**
 * Базовый класс персонажа
 */
public partial class Character : CharacterBody3D
{
	[Export] private float walkSpeed = 2.5f;
	[Export] private float runSpeed = 5.0f;
	[Export] private float acceleration = 0.2f;

	private Sprite3D sprite;

	public enum CharacterType
	{
		None,
		Guard,
		Thief
	}

	public readonly CharacterType type;

	public bool isRunning;
	public bool isSitting;

	public void UpdateMoveDirection(Vector3 direction)
	{
		var velocity = Velocity;
		var speed = isRunning ? runSpeed : walkSpeed;
		velocity.X = Mathf.MoveToward(Velocity.X, direction.X * speed, acceleration);
		velocity.Z = Mathf.MoveToward(Velocity.Z, direction.Z * speed, acceleration);
		Velocity = velocity;

		if (direction.X != 0)
		{
			sprite.FlipH = direction.X < 0;
		}
	}

	public override void _Ready()
	{
		sprite = GetNode<Sprite3D>("sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Velocity.Length() > 0)
		{
			MoveAndSlide();
		}
	}
}