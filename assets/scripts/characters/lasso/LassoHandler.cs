using Godot;
using RoyalCupcakes.Utils;

namespace RoyalCupcakes.Characters.Lasso;

/**
 * Спавнит лассо и отправляет в точку на курсоре
 */
public partial class LassoHandler : Node3D
{
	[Export] private PackedScene lassoPrefab;

	private Node3D lassoParent;
	
	private Sprite3D bodySprite;
	private Sprite3D mySprite;
	private bool isFlip;

	public Vector3 LassoPosition => mySprite.GlobalPosition;
	public Character PlayerOwner { get; private set; }

	public override void _EnterTree()
	{
		PlayerOwner = GetParent<Character>();
		SetMultiplayerAuthority(PlayerOwner.PlayerId);
	}

	public override void _Ready()
	{
		bodySprite = GetNode<Sprite3D>("../sprite");
		mySprite = GetNode<Sprite3D>("sprite");
		lassoParent = PlayerOwner.GetParent<Node3D>();
		
		CallDeferred(nameof(LoadPlayerTeam));
	}

	private async void LoadPlayerTeam()
	{
		await ToSignal(GetTree(), "process_frame");
		Visible = PlayerOwner.Team == Team.Guard;
	}

	public override void _Process(double delta)
	{
		if (!Visible) return;
		if (PlayerOwner.IsTied) return;
		UpdateFlip();

		if (!Multiplayer.HasMultiplayerPeer() || !IsMultiplayerAuthority()) return;
		
		if (!Input.IsActionJustPressed("ui_click")) return;
		var target = Cursor.GetCursorWorldPosition(this);
		Rpc(nameof(SpawnLasso), target);
	}

	private void UpdateFlip()
	{
		if (isFlip == bodySprite.FlipH) return;

		isFlip = bodySprite.FlipH;
		Scale = new Vector3(isFlip ? -1 : 1, Scale.Y, Scale.Z);
	}

	[Rpc(CallLocal = true)]
	private void SpawnLasso(Vector3 target)
	{
		if (target == Vector3.Zero) return;
		
		var lasso = lassoPrefab.Instantiate<Lasso>();
		lasso.Name = "lasso_" + PlayerOwner.PlayerId;
		lasso.LassoHandler = this;
		lassoParent.AddChild(lasso);
		lasso.GlobalPosition = LassoPosition;
		lasso.MoveToTarget(target);
		Visible = false;
	}
}
