using Godot;
using RoyalCupcakes.Characters.Lasso;
using RoyalCupcakes.Characters.Player;
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
	
	private const float walkSpeed = 8f;
	private const float runSpeed = 14f;
	private const float acceleration = 0.6f;
	private const float tiedAcceleration = 0.2f;
	private const float gravity = 2f;

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
		if (Multiplayer.IsServer())
		{
			LoadTeam((int)Team);
			LoadSprite(SpriteCode, false);
		}
		else
		{
			CallDeferred(nameof(LoadDeferred));
		}
	}

	private async void LoadDeferred()
	{
		await ToSignal(GetTree(), "process_frame");
		RpcId(1, nameof(RequestCharacterData));
	}

	public override void _Process(double delta)
	{
		if (IsTied) return;
		UpdateMoveDirection();
		UpdateFlipByDirection();
	}
	
	private void UpdateMoveDirection()
	{
		var speed = IsRunning ? runSpeed : walkSpeed;
		var direction = IsTied ? Vector3.Zero : Direction;
		if (direction.Length() > 0)
		{
			direction.Y = -gravity;
		}

		Velocity = Velocity.MoveToward(direction * speed, acceleration);
	}

	private void UpdateFlipByDirection()
	{
		var direction = IsTied ? Vector3.Zero : Direction;
		if (direction.Length() == 0) return;
		
		var rotatedDirection = GetRotatedSideDirection(direction);
		if (Mathf.Abs(rotatedDirection) < 0.1f) return;
		
		spriteLoader.FlipH = rotatedDirection < 0;
	}

	private float GetRotatedSideDirection(Vector3 direction)
	{
		return RotationDegrees.Y switch
		{
			>= 0 and < 90 => direction.X,
			>= 90 and < 180 => -direction.Z,
			>= 180 and < 270 => -direction.X,
			>= -90 and < 0 => direction.Z,
			>= -180 and < -90 => -direction.X,
			_ => 0
		};
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Velocity.Length() == 0) return;
		
		if (IsTied)
		{
			Velocity = Velocity.MoveToward(Vector3.Zero, tiedAcceleration);
		}
			
		MoveAndSlide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestCharacterData()
	{
		var senderId = Multiplayer.GetRemoteSenderId();
		RpcId(senderId, nameof(LoadTeam), (int)Team);
		RpcId(senderId, nameof(LoadSprite), SpriteCode, false);
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void LoadTeam(int newTeam)
	{
		Team = (Team)newTeam;

		var minimapItem = GetNodeOrNull<MinimapItem>("minimapItem");
		minimapItem?.Initialize();

		if ((Team)newTeam != Team.Guard) return;
		
		var lassoHandler = GetNode<LassoHandler>("lasso");
		lassoHandler.SetVisible();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void LoadSprite(string newCode, bool changeEffect)
	{
		SpriteCode = newCode;
		spriteLoader.Load();

		if (!changeEffect) return;
		
		var effect = changeEffectPrefab.Instantiate<Node3D>();
		effect.Rotation = Rotation;
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