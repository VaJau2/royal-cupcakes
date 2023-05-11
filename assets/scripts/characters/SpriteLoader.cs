using Godot;

namespace RoyalCupcakes.Characters;

/**
 * Загружает скин по коду
 */
public partial class SpriteLoader : Sprite3D
{
	private Character character;
	
	public override void _Ready()
	{
		character = GetParent<Character>();
	}

	public void Load()
	{
		var code = character.SpriteCode;
		if (string.IsNullOrEmpty(code)) return;
		
		var newTexture = GD.Load<Texture2D>($"res://assets/sprites/skins/{code}.png");
		Texture = newTexture;
	}
}
