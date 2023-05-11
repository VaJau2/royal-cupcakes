using Godot;

namespace RoyalCupcakes.Characters;

/**
 * Базовый класс персонажа
 */
public partial class Character : CharacterBody3D
{
	[Export] public string SpriteCode { get; set; }
	public bool IsRunning { get; set; }
	public bool IsSitting { get; set; }
	
	public enum CharacterType
	{
		None,
		Guard,
		Thief
	}
	
	public CharacterType Type { get; private set; }
	
	private const float walkSpeed = 2f;
	private const float runSpeed = 4f;
	private const float acceleration = 0.2f;

	private SpriteLoader spriteLoader;

	public void UpdateMoveDirection(Vector3 direction)
	{
		var speed = IsRunning ? runSpeed : walkSpeed;
		direction.Y = 0;
		Velocity = Velocity.MoveToward(direction * speed, acceleration);

		if (direction.X != 0)
		{
			spriteLoader.FlipH = direction.X < 0;
		}
	}

	public void LoadSpriteByCode(string code)
	{
		SpriteCode = code;
		spriteLoader.Load();
	}

	public override void _Ready()
	{
		spriteLoader = GetNode<SpriteLoader>("sprite");
		spriteLoader.Load();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Velocity.Length() > 0)
		{
			MoveAndSlide();
		}
	}
}