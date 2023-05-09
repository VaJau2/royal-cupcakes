using Godot;

namespace RoyalCupcakes.Characters;

/**
 * Отвечает за анимацию персонажа
 */
public partial class CharacterAnimator : AnimationPlayer
{
	private const float MinAnimSpeed = 0.1f;
	
	private Character character;

	private enum Animation
	{
		idle, walk, run, tied, sit
	}
	
	public override void _Ready()
	{
		character = GetParent<Character>();
	}
	
	public override void _Process(double delta)
	{
		var currentAnim = GetCurrentAnimation();
		CurrentAnimation = currentAnim.ToString();
	}

	private Animation GetCurrentAnimation()
	{
		if (character.Velocity.Length() > MinAnimSpeed)
		{
			return character.isRunning ? Animation.run : Animation.walk;
		}

		return character.isSitting ? Animation.sit : Animation.idle;
	}
}
