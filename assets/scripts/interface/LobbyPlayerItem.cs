using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class LobbyPlayerItem : Control
{
	[Export] public bool PlayerReady { get; set; }
	[Export] public Team Team { get; set; }
	[Export] public string PlayerName { get; set; }

	private LobbyMenu lobbyMenu;
	
	private Label playerNameLabel;
	private TextureRect teamIcon;
	private Texture2D[] teamTextures = new Texture2D[2];

	private TextureRect readyIcon;
	private Texture2D[] readyTextures = new Texture2D[2];

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(int.Parse(Name));
	}

	public override void _Ready()
	{
		lobbyMenu = GetNode<LobbyMenu>("../../../");
		
		playerNameLabel = GetNode<Label>("playerName");
		teamIcon = GetNode<TextureRect>("teamIcon");
		readyIcon = GetNode<TextureRect>("readyIcon");

		var path = "res://assets/sprites/interface/";
		teamTextures[0] = GD.Load<Texture2D>(path + "cake_icon.png");
		teamTextures[1] = GD.Load<Texture2D>(path + "shield_icon.png");

		readyTextures[0] = GD.Load<Texture2D>(path + "no_icon.png");
		readyTextures[1] = GD.Load<Texture2D>(path + "yes_icon.png");

		CallDeferred(nameof(SyncPlayerName));
	}

	public void SyncPlayerName()
	{
		if (IsMultiplayerAuthority())
		{
			SetPlayerName();
			OnSynchronize();
		}
		else
		{
			RpcId(int.Parse(Name), nameof(SetPlayerName));
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SetPlayerName()
	{
		if (IsMultiplayerAuthority())
		{
			PlayerName = Settings.Instance.PlayerName;
		}
	}

	public void OnSynchronize()
	{
		playerNameLabel.Text = PlayerName;
		
		var textureId = PlayerReady ? 1 : 0;
		readyIcon.Texture = readyTextures[textureId];
		
		teamIcon.Texture = teamTextures[(int)Team];

		lobbyMenu.CheckStartGame();
	}
}
