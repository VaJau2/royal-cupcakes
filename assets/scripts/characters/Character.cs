using Godot;

namespace RoyalCupcakes.Characters;

/**
 * Базовый класс персонажа
 */
public partial class Character : CharacterBody3D
{
	[Export] public string SpriteCode { get; set; }
	[Export] public bool IsRunning { get; set; }
	[Export] public bool IsSitting { get; set; }
	
	public Team Team { get; set; }
	
	private const float walkSpeed = 2f;
	private const float runSpeed = 4f;
	private const float acceleration = 0.2f;

	private SpriteLoader spriteLoader;

	public void UpdateMoveDirection(Vector3 direction)
	{
		if (!Multiplayer.HasMultiplayerPeer() || !IsMultiplayerAuthority()) return;
		
		var speed = IsRunning ? runSpeed : walkSpeed;
		direction.Y = 0;
		Velocity = Velocity.MoveToward(direction * speed, acceleration);

		if (direction.X != 0)
		{
			spriteLoader.FlipH = direction.X < 0;
		}
	}

	public override void _EnterTree()
	{
		var id = Name.ToString().Split('_')[1];
		SetMultiplayerAuthority(int.Parse(id));
	}

	public override void _Ready()
	{
		spriteLoader = GetNode<SpriteLoader>("sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!Multiplayer.HasMultiplayerPeer() || !IsMultiplayerAuthority()) return;
		
		if (Velocity.Length() > 0)
		{
			MoveAndSlide();
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void LoadTeam(int newTeam)
	{
		Team = (Team)newTeam;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void LoadSprite(string newCode)
	{
		SpriteCode = newCode;
		spriteLoader.Load();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SetGlobalPos(Vector3 newPos)
	{
		GlobalPosition = newPos;
	}
}