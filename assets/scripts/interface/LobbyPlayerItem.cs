using System.Collections.Generic;
using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class LobbyPlayerItem : Control
{
	[Export] public int PlayerId { get; private set; }
	[Export] public bool PlayerReady { get; set; }
	[Export] public Team Team { get; set; }
	[Export] public string PlayerName { get; private set; }

	private LobbyMenu lobbyMenu;
	private Main main;
	
	private Label playerNameLabel;
	private TextureRect teamIcon;
	private TextureRect readyIcon;
	
	private readonly Dictionary<Team, Texture2D> teamTextures = new();
	private readonly Dictionary<bool, Texture2D> readyTextures = new();

	public override void _EnterTree()
	{
		PlayerId = int.Parse(Name);
		SetMultiplayerAuthority(PlayerId);
	}

	public override void _Ready()
	{
		main = GetNode<Main>("/root/Main");
		lobbyMenu = GetNode<LobbyMenu>("../../../");
		
		playerNameLabel = GetNode<Label>("playerName");
		teamIcon = GetNode<TextureRect>("teamIcon");
		readyIcon = GetNode<TextureRect>("readyIcon");

		var path = "res://assets/sprites/interface/";
		teamTextures[Team.Thief] = GD.Load<Texture2D>(path + "cake_icon.png");
		teamTextures[Team.Guard] = GD.Load<Texture2D>(path + "shield_icon.png");

		readyTextures[false] = GD.Load<Texture2D>(path + "no_icon.png");
		readyTextures[true] = GD.Load<Texture2D>(path + "yes_icon.png");

		CallDeferred(nameof(SyncPlayerData));
	}

	public void SyncPlayerData()
	{
		if (!IsMultiplayerAuthority())
		{
			RpcId(PlayerId, nameof(RequestData));
		}
		else
		{
			PlayerName = Settings.Instance.PlayerName;

			if (main.PlayerTeam == Team.Npc)
			{
				main.PlayerTeam = Team.Thief;
			}
		
			Team = main.PlayerTeam;
		
			Synchronize();
		}
		
	}

	public void Synchronize()
	{
		Rpc(nameof(SendData), PlayerName, (int)Team, PlayerReady);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void RequestData()
	{
		var senderId = Multiplayer.GetRemoteSenderId();
		RpcId(senderId, nameof(SendData), PlayerName, (int)Team, PlayerReady);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void SendData(string playerName, int playerTeam, bool playerReady)
	{
		PlayerName = playerName;
		Team = (Team)playerTeam;
		PlayerReady = playerReady;

		playerNameLabel.Text = PlayerName;
		readyIcon.Texture = readyTextures[PlayerReady];

		if (teamTextures.ContainsKey(Team))
		{
			teamIcon.Texture = teamTextures[Team];
		}		
		
		lobbyMenu.CheckStartGame();
	}
}
