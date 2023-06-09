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
		if (!Multiplayer.HasMultiplayerPeer() || !character.IsMultiplayerAuthority()) return;
		var currentAnim = GetCurrentAnimation();
		CurrentAnimation = currentAnim.ToString();
	}

	private Animation GetCurrentAnimation()
	{
		if (character.IsTied) return Animation.tied;
		
		if (character.Velocity.Length() > MinAnimSpeed)
		{
			return character.IsRunning ? Animation.run : Animation.walk;
		}

		return character.IsSitting ? Animation.sit : Animation.idle;
	}
}
