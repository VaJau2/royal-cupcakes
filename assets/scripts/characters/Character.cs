using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Characters;

/**
 * Базовый класс персонажа
 */
public partial class Character : CharacterBody3D
{
	[Export] public string SpriteCode { get; set; }
	[Export] public bool IsRunning { get; set; }
	[Export] public bool IsSitting { get; set; }
	[Export] public bool IsTied { get; private set; }
	[Export] public Vector3 Direction { get; set; }

	[Export] private PackedScene changeEffectPrefab;
	
	public Team Team { get; set; }
	public int PlayerId { get; set; }
	public PlayersManager Manager => GetParent<PlayersManager>();
	
	private const float walkSpeed = 2f;
	private const float runSpeed = 4f;
	private const float acceleration = 0.2f;

	private SpriteLoader spriteLoader;
	
	[Signal]
	public delegate void TiedEventHandler();

	public override void _EnterTree()
	{
		PlayerId = int.Parse(Name.ToString().Split('_')[1]);
		SetMultiplayerAuthority(PlayerId);
	}

	public override void _Ready()
	{
		spriteLoader = GetNode<SpriteLoader>("sprite");
	}

	public override void _Process(double delta)
	{
		if (IsTied) return;
		UpdateMoveDirection();
	}
	
	private void UpdateMoveDirection()
	{
		var speed = IsRunning ? runSpeed : walkSpeed;
		var direction = Direction;
		direction.Y = 0;
		Velocity = Velocity.MoveToward(direction * speed, acceleration);

		if (direction.X != 0)
		{
			spriteLoader.FlipH = direction.X < 0;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Velocity.Length() == 0) return;
		
		if (IsTied)
		{
			Velocity = Velocity.MoveToward(Vector3.Zero, 0.05f);
		}
			
		MoveAndSlide();
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void LoadTeam(int newTeam)
	{
		Team = (Team)newTeam;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void LoadSprite(string newCode, bool changeEffect)
	{
		SpriteCode = newCode;
		spriteLoader.Load();

		if (!changeEffect) return;
		
		var effect = changeEffectPrefab.Instantiate<Node3D>();
		GetParent().AddChild(effect);
		effect.GlobalPosition = new Vector3(GlobalPosition.X, GlobalPosition.Y, GlobalPosition.Z + 0.02f);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SetGlobalPos(Vector3 newPos)
	{
		GlobalPosition = newPos;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SetIsTied(bool newTied)
	{
		IsTied = newTied;
		
		if (IsMultiplayerAuthority() && newTied)
		{
			EmitSignal(SignalName.Tied);
		}
	}
}