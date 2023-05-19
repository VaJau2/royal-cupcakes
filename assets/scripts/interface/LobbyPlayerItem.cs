using System.Collections.Generic;
using Godot;
using RoyalCupcakes.System;

namespace RoyalCupcakes.Interface;

public partial class LobbyPlayerItem : Control
{
	[Export] public bool PlayerReady { get; set; }
	[Export] public Team Team { get; set; }
	[Export] public string PlayerName { get; set; }

	private LobbyMenu lobbyMenu;
	private Main main;
	
	private Label playerNameLabel;
	private TextureRect teamIcon;
	private TextureRect readyIcon;
	
	private readonly Dictionary<Team, Texture2D> teamTextures = new();
	private readonly Dictionary<bool, Texture2D> readyTextures = new();

	public override void _EnterTree()
	{
		SetMultiplayerAuthority(int.Parse(Name));
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
		if (!IsMultiplayerAuthority()) return;
		PlayerName = Settings.Instance.PlayerName;
		Team = main.PlayerTeam;
		
		OnSynchronize();
	}

	public void OnSynchronize()
	{
		playerNameLabel.Text = PlayerName;
		readyIcon.Texture = readyTextures[PlayerReady];

		if (teamTextures.ContainsKey(Team))
		{
			teamIcon.Texture = teamTextures[Team];
		}		
		
		lobbyMenu.CheckStartGame();
	}
}
